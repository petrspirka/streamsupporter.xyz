using NewStreamSupporter.Contracts;
using NewStreamSupporter.Helpers;

namespace NewStreamSupporter.Models
{
    /// <summary>
    /// Model sloužící pro namapování nastavení moodulu Twitch z konfiguračního souboru aplikace
    /// </summary>
    public class TwitchOptions : ITwitchOptions
    {
        /// <summary>
        /// Uživatelské jméno účtu, který bude připojen do chatu
        /// </summary>
        public string ChatUsername { get; set; }
        /// <summary>
        /// Token uživatelského účtu, který bude připojen do chatu
        /// </summary>
        public string ChatToken { get; set; }
        /// <summary>
        /// Klientské Id aplikace
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// Klíč klienta aplikace
        /// </summary>
        public string ClientSecret { get; set; }
        /// <summary>
        /// Adresa, na kterou mají přicházet EventSub notifikace
        /// </summary>
        public Uri WebhookCallbackUri { get; set; }
        /// <summary>
        /// Klíč pro zabezpečení komunikace mezi platformou Twitch a aplikací
        /// </summary>
        public string WebhookSecret { get; set; }

        /// <summary>
        /// Vytvoří novou instancí třídy TwitchOptions. Je doporučeno generovat tuto třídu z konfiguračního souboru použitím metody <see cref="CreateFromSection(IConfigurationSection)"
        /// </summary>
        /// <param name="chatUsername">Uživatelské jméno účtu, který bude připojen do chatu</param>
        /// <param name="chatToken">Token uživatelského účtu, který bude připojen do chatu</param>
        /// <param name="clientId">Klientské Id aplikace</param>
        /// <param name="clientSecret">Klíč klienta aplikace</param>
        /// <param name="webhookCallbackUri">Adresa, na kterou mají přicházet EventSub notifikace</param>
        /// <param name="webhookSecret">Klíč pro zabezpečení komunikace mezi platformou Twitch a aplikací</param>
        public TwitchOptions(string chatUsername, string chatToken, string clientId, string clientSecret, Uri webhookCallbackUri, string webhookSecret)
        {
            ChatUsername = chatUsername;
            ChatToken = chatToken;
            ClientId = clientId;
            ClientSecret = clientSecret;
            WebhookCallbackUri = webhookCallbackUri;
            WebhookSecret = webhookSecret;
        }

        /// <summary>
        /// Metoda generující TwitchOptions z konfigurační sekce
        /// </summary>
        /// <param name="configurationSection">Konfigurační sekce získána z konfiguračního souboru aplikace</param>
        /// <returns>Instanci TwitchOptions</returns>
        /// <exception cref="ArgumentException">Pokud je v dané konfigurační sekci chyba</exception>
        public static TwitchOptions CreateFromSection(IConfigurationSection configurationSection)
        {
            string? clientId = configurationSection["ClientId"];
            string? clientSecret = configurationSection["ClientSecret"];
            string? chatUsername = configurationSection["ChatUsername"];
            string? chatToken = configurationSection["ChatToken"];
            string? webhookCallback = configurationSection["WebhookCallback"];
            string? webhookSecret = configurationSection["WebhookSecret"];

            //Kontrola všech hodnot sekce
            if (string.IsNullOrEmpty(clientId))
            {
                throw ExceptionHelper.GenerateMissingConfig(nameof(clientId));
            }

            if (string.IsNullOrEmpty(clientSecret))
            {
                throw ExceptionHelper.GenerateMissingConfig(nameof(clientSecret));
            }

            if (string.IsNullOrEmpty(chatUsername))
            {
                throw ExceptionHelper.GenerateMissingConfig(nameof(chatUsername));
            }

            if (string.IsNullOrEmpty(chatToken))
            {
                throw ExceptionHelper.GenerateMissingConfig(nameof(chatToken));
            }

            if (string.IsNullOrEmpty(webhookCallback))
            {
                throw ExceptionHelper.GenerateMissingConfig(nameof(webhookCallback));
            }

            //Kontrola, zda je Uri validní
            if (!Uri.IsWellFormedUriString(webhookCallback, UriKind.Absolute))
            {
                throw new ArgumentException($"The {nameof(WebhookCallbackUri)} field contains malformed URI \"{webhookCallback}\"");
            }

            if (string.IsNullOrEmpty(webhookSecret))
            {
                throw ExceptionHelper.GenerateMissingConfig(nameof(webhookSecret));
            }

            return new TwitchOptions(
                chatUsername,
                chatToken,
                clientId,
                clientSecret,
                new Uri(webhookCallback),
                webhookSecret);
        }
    }
}
