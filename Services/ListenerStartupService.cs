using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Contracts;
using NewStreamSupporter.Data;
using NewStreamSupporter.Models;

namespace NewStreamSupporter.Services
{
    /// <summary>
    /// Služba obstraávající inicializaci a spuštění služeb typu <see cref="BaseListenerService"/>.
    /// </summary>
    public class ListenerStartupService
    {
        private readonly TwitchListenerService _twitchService;
        private readonly YouTubeListenerService _youtubeService;
        private readonly ICurrencyService _currencyService;
        private readonly IServiceProvider _serviceProvider;
        private readonly DispatcherHubStateService _hubService;

        /// <summary>
        /// Vytvoří novou instanci třídy ListenerStartupService
        /// </summary>
        /// <param name="twitchService">Modul pro platformu Twitch</param>
        /// <param name="youtubeService">Modul pro platformu YouTube Live</param>
        /// <param name="currencyService">Služba pro správu bodů uživatelů</param>
        /// <param name="serviceProvider">Poskytovatel služeb</param>
        /// <param name="hub">DispatcherHub sloužící pro notifikace widgetům</param>
        public ListenerStartupService(TwitchListenerService twitchService, YouTubeListenerService youtubeService, ICurrencyService currencyService, IServiceProvider serviceProvider, DispatcherHubStateService hubService)
        {
            _twitchService = twitchService;
            _youtubeService = youtubeService;
            _currencyService = currencyService;
            _serviceProvider = serviceProvider;
            _hubService = hubService;
        }

        /// <summary>
        /// Metoda na registrování potřebných vazeb pro služby typu BaseListenerService.
        /// </summary>
        /// <returns></returns>
        public async Task RegisterListeners()
        {
            await _twitchService.InitializeAsync();
            await _youtubeService.InitializeAsync();
            await RegisterTwitchListeners();
            await RegisterYouTubeListeners();

            _currencyService.AddListenerService(_twitchService);
            _currencyService.AddListenerService(_youtubeService);

            //Eventy sloužící pro aktualizaci widgetů
            _twitchService.OnStreamDonation += OnStreamDonation;
            _youtubeService.OnStreamDonation += OnStreamDonation;

            _twitchService.OnStreamFollow += OnStreamFollow;
            _youtubeService.OnStreamFollow += OnStreamFollow;
        }

        /// <summary>
        /// Metoda pro upozornění na nového sledujícího
        /// </summary>
        /// <param name="sender">Objekt, který tuto událost vyvolal</param>
        /// <param name="e">Argument události</param>
        private async void OnStreamFollow(object? sender, StreamFollowEventArgs e)
        {
            using AsyncServiceScope scope = _serviceProvider.CreateAsyncScope();
            ApplicationContext context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();
            ApplicationUser? channelUser = await context.Users.Include(u => u.Alert).Where(u => e.User.Platform == Platform.Twitch ? u.TwitchId == e.Channel : u.GoogleBrandId == e.Channel).FirstOrDefaultAsync();
            if (channelUser == null || channelUser.Alert == null)
            {
                return;
            }

            //Pokud uživatel nakonfiguroval upozornění tak, aby mu přicházely notifikace o sledujících, spustíme toto upozornění
            if (channelUser.Alert.ShouldTriggerFollows)
            {
                await _hubService.Trigger("alert", channelUser.Alert.Id, $"{e.User.Name} is now following you!");
            }
            await notificationService.AddNotification(channelUser.Id, $"{e.User.Name} is now following you", "#008800FF");
        }

        /// <summary>
        /// Metoda pro upozornění na nový finanční příspěvek
        /// </summary>
        /// <param name="sender">Objekt, který tuto událost vyvolal</param>
        /// <param name="e">Argumenty události</param>
        private async void OnStreamDonation(object? sender, StreamDonationEventArgs e)
        {
            using AsyncServiceScope scope = _serviceProvider.CreateAsyncScope();
            ApplicationContext context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();
            ApplicationUser? channelUser = await context.Users.Include(u => u.Alert).Where(u => e.User.Platform == Platform.Twitch ? u.TwitchId == e.Channel : u.GoogleBrandId == e.Channel).FirstOrDefaultAsync();
            if (channelUser == null)
            {
                return;
            }

            //Zvýsíme momentální přispěnou částku u všech cílů příspěvků uživatele a spustíme notifikaci
            List<DonationGoalModel> goals = await context.DonationGoalModel.Where(d => d.OwnerId == channelUser.Id).ToListAsync();
            foreach (DonationGoalModel goal in goals)
            {
                await _hubService.Trigger("donationGoal", goal.Id, $"{e.Amount:n2}:{e.User.Name}");
                goal.CurrentAmount += decimal.Parse(Math.Round(e.Amount, 2).ToString());
            }
            await context.SaveChangesAsync();

            //Pokud uživatel chce dostávat upozornění ohledně příspěvků, spustíme upozornění s informacemi o uživatelim který přispěl
            if (channelUser.Alert != null && channelUser.Alert.ShouldTriggerDonations)
            {
                await _hubService.Trigger("alert", channelUser.Alert.Id, $"{e.User.Name} donated {e.Amount:c2}");
            }

            await notificationService.AddNotification(channelUser.Id, $"{e.User.Name} donated {e.Amount:n2}", "#008800FF");
        }

        /// <summary>
        /// Metoda sloužící pro registraci posluchačů platformy Twitch
        /// </summary>
        private async Task RegisterTwitchListeners()
        {
            using AsyncServiceScope scope = _serviceProvider.CreateAsyncScope();
            ApplicationContext context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            NotificationService notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();

            //Vybereme všechny uživatele, kteří mají aktivní Twitch a mají propojený účet s Twitchem
            List<ApplicationUser> validUsers = await context.Users
                .Where(u => u.IsTwitchActive)
                .Where(u => u.TwitchId != null)
                .ToListAsync();

            //Vybereme všechny uživatele, u kterých selhala registrace odběrů a upozorníme je pomocí notifikace
            IList<string> invalidTwitchUsers = await RegisterServiceListeners(validUsers.Select(u => u.TwitchId!).ToList(), _twitchService);
            IEnumerable<ApplicationUser> invalidUsers = validUsers.Where(u => invalidTwitchUsers.Contains(u.TwitchId!));

            foreach (ApplicationUser? user in invalidUsers)
            {
                await notificationService.AddNotification(user.Id, "Could not create Twitch subscriptions for your account, try to remove and add the external login from account settings.", "#FF0000FF");
            }
        }

        /// <summary>
        /// Metoda sloužící pro registraci posluchačů platformy YouTube Live
        /// </summary>
        private async Task RegisterYouTubeListeners()
        {
            using AsyncServiceScope scope = _serviceProvider.CreateAsyncScope();
            ApplicationContext context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            NotificationService notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();

            //Opět vybíráme jen ty uživatele, kteří aktivovali YouTube Live v nastavení a mají propojený účet
            List<ApplicationUser> validUsers = await context.Users
                .Where(u => u.IsGoogleActive)
                .Where(u => u.GoogleBrandId != null)
                .ToListAsync();

            //Opět vybereme uživatele u kterých registrace selhala a pošleme jim notifikaci
            IList<string> invalidGoogleUsers = await RegisterServiceListeners(validUsers.Select(u => u.GoogleBrandId!).ToList(), _youtubeService);
            IEnumerable<ApplicationUser> invalidUsers = validUsers.Where(u => invalidGoogleUsers.Contains(u.GoogleBrandId!));

            foreach (ApplicationUser? user in invalidUsers)
            {
                await notificationService.AddNotification(user.Id, "Could not create YouTube subscriptions for your account, try to remove and add the external login from account settings.", "#FF0000FF");
            }
        }

        /// <summary>
        /// Pomocná metoda pro registraci všech uživatelů v seznamu
        /// </summary>
        /// <param name="users">Uživatelé, kteří mají být zaregistrováni.</param>
        /// <param name="service">Služba, ve které uživatele registrujeme</param>
        /// <returns>Seznam uživatelů, u kterých registrace selhala</returns>
        private static async Task<IList<string>> RegisterServiceListeners(List<string> users, BaseListenerService service)
        {
            List<string> invalidUsers = new();
            foreach (string user in users)
            {
                StreamEventType? invalidType = await service.AddAllUserListeners(user);
                if (invalidType != null)
                {
                    invalidUsers.Add(user);
                }
            }
            return invalidUsers;
        }
    }
}
