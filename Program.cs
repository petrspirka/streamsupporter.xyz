using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NewStreamSupporter.Contracts;
using NewStreamSupporter.Data;
using NewStreamSupporter.Filters;
using NewStreamSupporter.Helpers;
using NewStreamSupporter.Models;
using NewStreamSupporter.Services;
using NewStreamSupporter.Services.Twitch;
using NewStreamSupporter.Services.YouTube;
using OpenExchangeRates;
using TwitchLib.Api;
using TwitchLib.Api.Core;
using TwitchLib.Api.Core.Interfaces;
using TwitchLib.Api.Interfaces;
using TwitchLib.Client;
using TwitchLib.Client.Interfaces;
using TwitchLib.Communication.Interfaces;
using TwitchLib.EventSub.Webhooks.Extensions;

namespace NewStreamSupporter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            string connectionString = builder.Configuration.GetConnectionString("ApplicationContextConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationContextConnection' not found.");

            builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlite(connectionString));

            //Originally the application had multiple locales it could use. This was scraped later down the line but all the required code is still present
            //If someone wants to implement view localization, simply inject IViewLocalizer into your views and create Resource files as seen in MS docs
            RequestLocalizationOptions localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(CultureHelper.Cultures[0].IetfLanguageTag)
                .AddSupportedCultures(CultureHelper.Cultures.Select(culture => culture.IetfLanguageTag).ToArray())
                .AddSupportedUICultures(CultureHelper.Cultures.Select(culture => culture.IetfLanguageTag).ToArray());

            ConfigurationManager config = builder.Configuration;
            if (config == null)
            {
                Console.WriteLine("Could not load the app configuration");
                return;
            }

            //Pøidání systému autentizace
            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;

            }).AddEntityFrameworkStores<ApplicationContext>();

            //Získáme konfiguraèní sekce
            IConfigurationSection twitchAuth = config.GetRequiredSection("Auth:Twitch");
            IConfigurationSection googleAuth = config.GetRequiredSection("Auth:Google");
            IConfigurationSection rewardModule = config.GetRequiredSection("RewardModule");

            //Kontrola validity WebhookCallback
            string twitchUriString = twitchAuth["WebhookCallback"]!;
            Uri twitchUri = new(twitchUriString);
            if (!twitchUri.IsDefaultPort || twitchUri.Scheme != "https")
            {
                throw new ArgumentException("Twitch webhooks have to use https scheme with default 443 port.");
            }

            //Pøidání SignalR
            builder.Services.AddSignalR(configure =>
            {
                configure.AddFilter<ValidClientFilter>();
            });

            //Pøídání extérních pøihlášení
            builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = googleAuth["ClientId"]!;
                    options.ClientSecret = googleAuth["ClientSecret"]!;
                    options.AccessType = "offline";
                    options.SaveTokens = true;
                    options.Scope.Add("https://www.googleapis.com/auth/youtube.readonly");
                    options.AuthorizationEndpoint += "?prompt=select_account";
                })
                .AddTwitch(options =>
                {
                    options.ClientId = twitchAuth["ClientId"]!;
                    options.ClientSecret = twitchAuth["ClientSecret"]!;
                    options.Scope.Add("bits:read");
                    options.Scope.Add("channel:read:subscriptions");
                    options.Scope.Add("moderator:read:followers");
                    options.SaveTokens = true;
                });

            builder.Services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });

            //Pøidání cookie policy
            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.CheckConsentNeeded = context => true;
            });

            builder.Services.AddRazorPages(options =>
            {
                //Vynutíme pøístup pouze autentizovaným uživatelùm do této oblasti
                options.Conventions.AuthorizeAreaFolder("Dashboard", "/");
            })
                .AddViewLocalization(options =>
                {
                    options.ResourcesPath = "Resources";
                });

            string twitchWebhookCallbackPath = twitchUri.PathAndQuery;

            //Pøidání EventSub Webhooku
            builder.Services.AddTwitchLibEventSubWebhooks(config =>
            {
                config.CallbackPath = twitchWebhookCallbackPath;
                config.Secret = twitchAuth["WebhookSecret"]!;
                config.EnableLogging = true;
            });

            //Registrace služeb aplikace
            builder.Services
                .AddTransient<ITwitchOptions>(impl => TwitchOptions.CreateFromSection(twitchAuth))
                .AddTransient<IYouTubeOptions>(impl => YouTubeOptions.CreateFromSection(googleAuth))
                .AddTransient<IRewardOptions>(impl => RewardOptions.CreateFromSection(rewardModule))
                .AddSingleton<ITwitchEventSubManager, TwitchEventSubManager>()
                .AddSingleton<ITwitchChatClient, TwitchChatClient>()
                .AddSingleton<ITwitchEventSubWebhookReceiver, TwitchEventSubWebhookReceiver>()
                .AddSingleton<TwitchListenerService>()
                .AddSingleton<ListenerStartupService>()
                .AddTransient<RewardManagerService>()
                .AddTransient<OpenExchangeRatesClient>(impl => new OpenExchangeRatesClient(config["OpenExchangeRatesAppId"]!))
                .AddSingleton<IYouTubePollingService, YouTubePollingService>()
                .AddTransient<YouTubeService>()
                .AddSingleton<ICurrencyService, CurrencyService>()
                .AddSingleton<ValidClientFilter>()
                .AddSingleton<DispatcherHubStateService>()
                .AddTransient<DispatcherHub>()
                .AddTransient<NotificationService>()
                .AddTransient<IDataStore>(impl => new FileDataStore(googleAuth["DataStoreFolderPath"]!, bool.Parse(googleAuth["ShouldDataStoreUseFullFolderPath"]!)))
                .AddTransient<IYouTubeProviderService, YouTubeProviderService>()
                .AddSingleton<YouTubeListenerService>()
                .AddHostedService(impl => impl.GetRequiredService<TwitchListenerService>())
                .AddHostedService(impl => impl.GetRequiredService<YouTubeListenerService>())
                .AddSingleton<ITwitchClient, TwitchClient>(impl =>
                {
                    TwitchClient twitchClient = new(impl.GetService<IClient>(), logger: impl.GetService<ILogger<TwitchClient>>());
                    using IServiceScope scope = impl.CreateScope();
                    ITwitchOptions config = scope.ServiceProvider.GetRequiredService<ITwitchOptions>();
                    twitchClient.Initialize(new(config.ChatUsername, config.ChatToken));
                    return twitchClient;
                })
                .AddSingleton<ITwitchAPI>(impl =>
                {
                    using IServiceScope scope = impl.CreateScope();
                    ITwitchOptions options = scope.ServiceProvider.GetRequiredService<ITwitchOptions>();
                    ApiSettings settings = new()
                    {
                        ClientId = options.ClientId,
                        Secret = options.ClientSecret
                    };
                    return new TwitchAPI(impl.GetService<ILoggerFactory>(), impl.GetService<IRateLimiter>(), settings, impl.GetService<IHttpCallHandler>());
                });

            //Pokud existuje validní konfigurace SMTP, zaregistrujeme SMTP službu
            IConfigurationSection smtpAuth = config.GetSection("SMTP");
            if (smtpAuth != null &&
                !smtpAuth["Email"].IsNullOrEmpty() &&
                !smtpAuth["Password"].IsNullOrEmpty() &&
                !smtpAuth["Host"].IsNullOrEmpty() &&
                !smtpAuth["Port"].IsNullOrEmpty())
            {

                builder.Services.AddSingleton<IEmailSender>(impl => new SMTPMailSender(smtpAuth["Email"]!, smtpAuth["Password"]!, smtpAuth["Host"]!, int.Parse(smtpAuth["Port"]!)));
            }

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();

            app.MapAreaControllerRoute("apiControllers", "Api", "{area}/{controller}/{action}");

            app.UseRequestLocalization(localizationOptions);

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseTwitchLibEventSubWebhooks();

            app.MapRazorPages();
            app.MapHub<DispatcherHub>("/Api/Dispatcher");

            IEmailSender? mailService = app.Services.GetService<IEmailSender>();
            if (mailService != null && mailService is IHostedService hostedMailService)
            {
                hostedMailService.StartAsync(new CancellationToken());
            }

            //Inicializace hostovaných služeb
            InitializeHostedServices(app.Services).GetAwaiter().GetResult();

            app.Run();
        }

        private static async Task InitializeHostedServices(IServiceProvider provider)
        {
            using AsyncServiceScope scope = provider.CreateAsyncScope();
            ListenerStartupService initService = scope.ServiceProvider.GetRequiredService<ListenerStartupService>();
            await initService.RegisterListeners();
        }
    }
}