using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3.Data;
using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Contracts;
using NewStreamSupporter.Models;
using OpenExchangeRates;

namespace NewStreamSupporter.Services.YouTube
{
    /// <summary>
    /// Implementace služby <see cref="IYouTubePollingService"/>
    /// </summary>
    public class YouTubePollingService : IYouTubePollingService
    {

        /// <inheritdoc/>
        public event EventHandler<StreamChatMessageEventArgs>? OnStreamChatMessage;
        /// <inheritdoc/>
        public event EventHandler<StreamDonationEventArgs>? OnStreamDonation;
        /// <inheritdoc/>
        public event EventHandler<StreamEndedEventArgs>? OnStreamDown;
        /// <inheritdoc/>
        public event EventHandler<StreamStartedEventArgs>? OnStreamUp;
        /// <inheritdoc/>
        public event EventHandler<StreamFollowEventArgs>? OnStreamFollow;


        private readonly IYouTubeProviderService _youTubeProviderService;
        private readonly uint _streamPollingTime;
        private readonly uint _chatPollingTime;
        private readonly Timer _streamTimer;
        private readonly Timer _chatTimer;

        //Uchovává uživatele, kteří nestreamují a kteří streamují
        private readonly IList<string> _inactiveUsers;
        private readonly IDictionary<string, Tuple<string, string?>> _activeUsers;
        private readonly IServiceProvider _serviceProvider;

        public YouTubePollingService(IYouTubeOptions youtubeSettings, IYouTubeProviderService youTubeProviderService, IServiceProvider serviceProvider)
        {
            _youTubeProviderService = youTubeProviderService;
            _serviceProvider = serviceProvider;

            _streamPollingTime = youtubeSettings.StreamPollingTime;
            _chatPollingTime = youtubeSettings.ChatPollingTime;

            _inactiveUsers = new List<string>();
            _activeUsers = new Dictionary<string, Tuple<string, string?>>();

            _streamTimer = new Timer(OnStreamTimerTick);
            _chatTimer = new Timer(OnChatTimerTick);
        }

        /// <inheritdoc/>
        public void AddUserListener(string userId)
        {
            if (!_inactiveUsers.Contains(userId))
            {
                _inactiveUsers.Add(userId);
            }
        }

        /// <inheritdoc/>
        public void RemoveUserListener(string userId)
        {
            _activeUsers.Remove(userId);
            _inactiveUsers.Remove(userId);
        }

        /// <summary>
        /// Metoda spuštěna při tiku časovače pro streamy
        /// </summary>
        private async void OnStreamTimerTick(object? state)
        {
            string[] users = _inactiveUsers.ToArray();
            foreach (string user in users)
            {
                IEnumerable<LiveBroadcast>? streams;
                try
                {
                    streams = await _youTubeProviderService.GetActiveStreamsForUser(user);
                }
                catch (UnauthorizedAccessException)
                {
                    await HandleUnauthorized(user);
                    continue;
                }
                if (streams == null)
                {
                    continue;
                }
                LiveBroadcast? stream = streams.FirstOrDefault();
                if (stream != null)
                {
                    OnStreamUp?.Invoke(this, new(user));
                    _activeUsers.Add(user, new(stream.Snippet.LiveChatId, null));
                    _inactiveUsers.Remove(user);
                }
            }
        }

        /// <summary>
        /// Metoda spuštěna, pokud uživatel zruší autorizaci aplikaci
        /// </summary>
        /// <param name="userId">Uživatelské Id v rámci platformy</param>
        private async Task HandleUnauthorized(string userId)
        {
            _activeUsers.Remove(userId);
            _inactiveUsers.Remove(userId);
            using AsyncServiceScope scope = _serviceProvider.CreateAsyncScope();
            Data.ApplicationContext context = scope.ServiceProvider.GetRequiredService<Data.ApplicationContext>();
            IDataStore store = scope.ServiceProvider.GetRequiredService<IDataStore>();
            NotificationService notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();
            Data.ApplicationUser? user = await context.Users.Where(u => u.GoogleBrandId == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                return;
            }
            await notificationService.AddNotification(user.Id, "Failed to connect to your YouTube account. Please try removing and adding it in user settings.", "#FF0000FF");
            user.GoogleBrandId = null;
            user.GoogleBrandName = null;
            user.IsGoogleActive = false;
            await store.DeleteAsync<TokenResponse>(userId);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Metoda spuštěná při tiku časovače pro chat
        /// </summary>
        private async void OnChatTimerTick(object? state)
        {
            using AsyncServiceScope scope = _serviceProvider.CreateAsyncScope();
            OpenExchangeRatesClient exchangeRatesClient = scope.ServiceProvider.GetRequiredService<OpenExchangeRatesClient>();

            Dictionary<string, Tuple<string, string?>> userChatPairs = _activeUsers.ToDictionary(i => i.Key, i => i.Value);
            foreach (KeyValuePair<string, Tuple<string, string?>> userChatPair in userChatPairs)
            {
                LiveChatMessageListResponse? messages;
                try
                {
                    //Pokusíme se získat zprávy
                    messages = await _youTubeProviderService.GetChatMessagesForUser(userChatPair.Key, userChatPair.Value.Item1, userChatPair.Value.Item2);
                }
                catch (UnauthorizedAccessException)
                {
                    await HandleUnauthorized(userChatPair.Key);
                    continue;
                }
                if (messages == null)
                {
                    continue;
                }

                bool streamEnded = false;
                foreach (LiveChatMessage? message in messages.Items)
                {
                    //Pokud bude libovolná zpráva ukončující, nastaví se streamEnded na true (logický OR)
                    streamEnded |= await ProcessMessage(userChatPair.Key, message, exchangeRatesClient);
                }
                _activeUsers[userChatPair.Key] = new(userChatPair.Value.Item1, messages.NextPageToken);

                //Kontrola ukončení streamu
                IEnumerable<LiveBroadcast>? streamRequest = await _youTubeProviderService.GetActiveStreamsForUser(userChatPair.Key);
                if (streamRequest == null || streamRequest.Count() == 0)
                {
                    streamEnded = true;
                }

                if (streamEnded)
                {
                    OnStreamDown?.Invoke(this, new(userChatPair.Key));
                    _inactiveUsers.Add(userChatPair.Key);
                    _activeUsers.Remove(userChatPair.Key);
                }
            }
        }

        //Pomocná metoda pro zpracování zprávy
        private async Task<bool> ProcessMessage(string userId, LiveChatMessage message, OpenExchangeRatesClient exchangeRatesClient)
        {
            LiveChatMessageSnippet snippet = message.Snippet;

            PlatformUser author = new(snippet.AuthorChannelId, message.AuthorDetails.DisplayName, Platform.YouTube);
            ConvertResponse? dollarAmount = null;
            switch (snippet.Type)
            {
                case "chatEndedEvent":
                    return true;
                case "superChatEvent":
                    dollarAmount = await exchangeRatesClient.ConvertAsync(
                        snippet.SuperChatDetails.Currency, "USD",
                        (decimal)(snippet.SuperChatDetails.AmountMicros ?? throw new ArgumentException("AmountMicros was null")))
                        ?? throw new Exception("The exchange rates client failed to convert currency correctly");

                    OnStreamDonation?.Invoke(this, new StreamDonationEventArgs(userId, author, (float)dollarAmount.Amount, snippet.SuperChatDetails.UserComment));
                    break;
                case "superStickerEvent":
                    dollarAmount = await exchangeRatesClient.ConvertAsync(
                        snippet.SuperStickerDetails.Currency, "USD",
                        (decimal)(snippet.SuperStickerDetails.AmountMicros ?? throw new ArgumentException("AmountMicros was null")))
                        ?? throw new Exception("The exchange rates client failed to convert currency correctly");

                    OnStreamDonation?.Invoke(this, new StreamDonationEventArgs(userId, author, (float)dollarAmount.Amount));
                    break;
                case "textMessageEvent":
                    OnStreamChatMessage?.Invoke(this, new StreamChatMessageEventArgs(userId, author, snippet.TextMessageDetails.MessageText));
                    break;

                case "membershipGiftingEvent":
                    //YouTube API currently does not support getting the cost of membership tiers, so it is impossible to determine how much was actually donated
                    //Until the API is updated, this event is gonna be left blank
                    break;
            }
            return false;
        }

        /// <inheritdoc/>
        public Task StartAsync(CancellationToken token)
        {
            _streamTimer.Change(0, _streamPollingTime);
            _chatTimer.Change(0, _chatPollingTime);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task StopAsync(CancellationToken token)
        {
            _streamTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _chatTimer.Change(Timeout.Infinite, Timeout.Infinite);
            return Task.CompletedTask;
        }
    }
}
