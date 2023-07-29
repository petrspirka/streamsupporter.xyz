using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Contracts;
using NewStreamSupporter.Data;
using NewStreamSupporter.Helpers;
using NewStreamSupporter.Models;
using TwitchLib.Api.Helix.Models.EventSub;
using TwitchLib.Api.Interfaces;

namespace NewStreamSupporter.Services
{
    /// <summary>
    /// Implementace <see cref="BaseListenerService"/> pro platformu Twitch.
    /// </summary>
    public class TwitchListenerService : BaseListenerService
    {
        //Reference na služby používané touto třídou
        private readonly ITwitchEventSubManager _eventSubManager;
        private readonly ITwitchChatClient _chatClient;
        private readonly ITwitchAPI _twitchApi;
        private readonly IServiceProvider _serviceProvider;

        /// <inheritdoc/>
        public override event EventHandler<StreamChatMessageEventArgs>? OnStreamChatMessage;
        /// <inheritdoc/>
        public override event EventHandler<StreamDonationEventArgs>? OnStreamDonation;
        /// <inheritdoc/>
        public override event EventHandler<StreamEndedEventArgs>? OnStreamDown;
        /// <inheritdoc/>
        public override event EventHandler<StreamStartedEventArgs>? OnStreamUp;
        /// <inheritdoc/>
        public override event EventHandler<StreamFollowEventArgs>? OnStreamFollow;

        //Uchovává informace o aktivních streamech
        private readonly IList<string> _activeStreams;
        //Uchovává informace o již sledujících uživatelech. Klíčem je Id streamera, hodnotou jsou jeho sledující uchování od posledního restartu aplikace
        private readonly IDictionary<string, IList<string>?> _existingFollows;

        /// <summary>
        /// Vytvoří novou instanci třídy TwitchListenerService
        /// </summary>
        /// <param name="eventSubManager">Služba pro správu EventSub odběrů</param>
        /// <param name="chatClient">Služba pro připojení chatovacího klienta</param>
        /// <param name="webhookReceiver">Služba pro získávání notifikací z EventSub</param>
        /// <param name="serviceProvider">Poskytovatel služeb pro získání scoped služeb</param>
        public TwitchListenerService(ITwitchEventSubManager eventSubManager, ITwitchChatClient chatClient, ITwitchEventSubWebhookReceiver webhookReceiver, ITwitchAPI twitchApi, IServiceProvider serviceProvider)
        {
            _eventSubManager = eventSubManager;
            _chatClient = chatClient;
            _twitchApi = twitchApi;
            _serviceProvider = serviceProvider;

            _activeStreams = new List<string>();
            _existingFollows = new Dictionary<string, IList<string>?>();

            //Namapování interních událostí na extérní
            webhookReceiver.OnStreamDown += (sender, e) =>
            {
                //Odstranění aktivního kanálu z cache
                _activeStreams.Remove(e.Channel);
                OnStreamDown?.Invoke(sender, e);
            };
            webhookReceiver.OnStreamUp += (sender, e) =>
            {
                //Přidání aktivního kanálu do cache
                _activeStreams.Add(e.Channel);
                OnStreamUp?.Invoke(sender, e);
            };
            webhookReceiver.OnStreamFollow += (sender, e) =>
            {
                //Kontrola, zda uživatel již nebyl sledujícím
                if (!_existingFollows.ContainsKey(e.Channel))
                {
                    _existingFollows[e.Channel] = new List<string>();
                }
                if (!_existingFollows[e.Channel]!.Contains(e.User.Id))
                {
                    OnStreamFollow?.Invoke(sender, e);
                    _existingFollows[e.Channel]!.Add(e.User.Id);
                }
            };
            webhookReceiver.OnStreamDonation += (sender, e) => OnStreamDonation?.Invoke(sender, e);
            webhookReceiver.OnUserRevocation += OnUserRevocation;
            _chatClient.StreamChatMessageReceived += async (sender, e) =>
            {
                string? channelId = await GetUserId(e.Channel);
                //Pokud uživatel momentálně nestreamuje, ignorujeme zprávy
                if (channelId != null && _activeStreams.Contains(channelId))
                {
                    //Adjust args to use userId instead of userName
                    StreamChatMessageEventArgs newArgs = new(channelId, e.User, e.Message);
                    OnStreamChatMessage?.Invoke(sender, newArgs);
                }
            };
        }

        /// <summary>
        /// Metoda, která se spustí pokud uživatel zruší přístup aplikace k jeho Twitch účtu
        /// </summary>
        /// <param name="sender">Instance třídy, která tuto událost vyvolala</param>
        /// <param name="e">Argument události</param>
        private async void OnUserRevocation(object? sender, UserRevocationEventArgs e)
        {
            //Vytvoření scope pro práci se scopovanými službami
            using IServiceScope scope = _serviceProvider.CreateScope();
            NotificationService notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();
            ApplicationContext context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            //Zrušení všech události pro uživatele
            await base.RemoveAllUserListeners(e.User.Id);
            ApplicationUser? user = await context.Users.Where(u => u.TwitchId == e.User.Id).FirstOrDefaultAsync();
            if (user == null)
            {
                return;
            }

            //Oznámení skutečnosti uživateli
            await notificationService.AddNotification(user.Id, "Failed to connect with your Twitch account, please try reconnecting your account from settings.", "#FF0000FF");
            user.TwitchId = null;
            user.TwitchUsername = null;
            user.TwitchAccessToken = null;
            user.TwitchRefreshToken = null;
            user.TwitchAccessTokenExpiry = null;
            await context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public override async Task<bool> AddUserListener(string userId, StreamEventType eventType)
        {
            if (eventType == StreamEventType.StreamMessage)
            {
                string? userName = await GetUserName(userId);
                if (userName == null)
                {
                    return false;
                }
                _chatClient.JoinChannel(userName);
            }
            else
            {
                foreach (EventSubSubscriptionDetails detail in eventType.GetEventSubSubscriptionsDetails(userId))
                {
                    bool created = await _eventSubManager.CreateSubscription(detail.Topic, detail.Condition, detail.Version);
                    if (!created)
                    {
                        return false;
                    }

                    if(detail.Topic == "stream.online")
                    {
                        var broadcasterId = detail.Condition["broadcaster_user_id"];
                        var streamResponse = await _twitchApi.Helix.Streams.GetStreamsAsync(userIds: new()
                        {
                            { broadcasterId }
                        });
                        if(streamResponse.Streams.Length != 0)
                        {
                            _activeStreams.Add(broadcasterId);
                        }
                    }

                    if (detail.Topic == "stream.offline")
                    {
                        var broadcasterId = detail.Condition["broadcaster_user_id"];
                        var streamResponse = await _twitchApi.Helix.Streams.GetStreamsAsync(userIds: new()
                        {
                            { broadcasterId }
                        });
                        if (streamResponse.Streams.Length == 0)
                        {
                            _activeStreams.Remove(broadcasterId);
                        }
                    }
                }
            }

            return true;
        }

        /// <inheritdoc/>
        public override async Task<bool> RemoveUserListener(string userId, StreamEventType eventType, bool bypassExistenceCheck = false)
        {
            if (eventType == StreamEventType.StreamMessage)
            {
                //Zprávy jsou prijímány z instance TwitchChatClient, tedy je musíme zrušit v této třídě
                string? userName = await GetUserName(userId);
                if (userName == null)
                {
                    return false;
                }
                _chatClient.LeaveChannel(userName);
            }
            else
            {
                //Ostatní události jsou prijímány z instance TwitchEventSubWebhookReceiver. Pro zrušení EventSub odběrů slouží TwitchEventSubManager
                IEnumerable<EventSubSubscription> subscriptions = _eventSubManager.GetSubscriptionsForUser(userId).ToList();

                //Kontrola, zda je zrušení odběrů validní
                bool valid = true;
                foreach (EventSubSubscription subscription in subscriptions)
                {
                    //Pokud je vráceno false, valid se nastaví na 0 (operátor AND)
                    valid &= await _eventSubManager.DeleteSubscription(subscription.Id, bypassExistenceCheck);
                }
                return valid;
            }
            return true;
        }

        /// <inheritdoc/>
        public override async Task InitializeAsync()
        {
            await _eventSubManager.InitializeAsync();
        }

        /// <inheritdoc/>
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _chatClient.StartAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _chatClient.StopAsync(cancellationToken);
        }

        /// <summary>
        /// Metoda umožňující získání Twitch uživatelského jména z Twitch Id
        /// </summary>
        /// <param name="userId">Uživatelské Id na platformě Twitch</param>
        /// <returns>Uživatelské jméno na platformě Twitch, pokud je zaznamenáno v databázi</returns>
        private async Task<string?> GetUserName(string userId)
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            ApplicationContext context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            ApplicationUser? user = await context.Users.Where(u => u.TwitchId == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                return null;
            }
            return user.TwitchUsername!;
        }

        private async Task<string?> GetUserId(string userName)
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            ApplicationContext context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            ApplicationUser? user = await context.Users.Where(u => u.TwitchUsername!.ToLower() == userName.ToLower()).FirstOrDefaultAsync();
            if (user == null)
            {
                return null;
            }
            return user.TwitchId!;
        }
    }
}
