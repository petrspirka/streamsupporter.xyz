using NewStreamSupporter.Contracts;
using NewStreamSupporter.Helpers;

namespace NewStreamSupporter.Models
{
    public class YouTubeOptions : IYouTubeOptions
    {
        /// <summary>
        /// Klientské Id aplikace
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// Klíč klienta aplikace
        /// </summary>
        public string ClientSecret { get; set; }
        /// <summary>
        /// Cesta k úložišti YouTube tokenů. Pokud není parametr <see cref="ShouldDataStoreUseFullPath"/> false, cesta je relativní vůči adresáři AppData aplikace
        /// </summary>
        public string DataStoreFolderPath { get; set; }
        /// <summary>
        /// Zda je DataStoreFolderPath cesta absolutní nebo relativní vůči složce aplikace v AppData 
        /// </summary>
        public bool ShouldDataStoreUseFullPath { get; set; }
        /// <summary>
        /// Interval pollování endpointu pro Streamy
        /// </summary>
        public uint StreamPollingTime { get; set; }
        /// <summary>
        /// Interval pollování endpointu pro Chat streamu
        /// </summary>
        public uint ChatPollingTime { get; set; }

        /// <summary>
        /// Vytvoří novou instanci třídy YouTubeOptions. Je doporučeno generovat tuto třídu z konfiguračního souboru použitím metody <see cref="CreateFromSection(IConfigurationSection)"/>.
        /// </summary>
        /// <param name="clientId">Klientské Id aplikace</param>
        /// <param name="clientSecret">Klíč klienta aplikace</param>
        /// <param name="dataStoreFolderPath">Cesta k úložisti YouTube tokenů. Pokud je parametr <paramref name="dataStoreFolderPath"/> true, bude cesta absolutní, pokud ne, bude cesta vůci adresáři AppData aplikace</param>
        /// <param name="shouldDataStoreUseFullPath">Zda je DataStoreFolderPath cesta absolutní nebo relativní vůči složce aplikace v AppData </param>
        /// <param name="streamPollingTime">Interval pollování endpointu pro Streamy</param>
        /// <param name="chatPollingTime">Interval pollování endpointu pro Chat streamu</param>
        public YouTubeOptions(string clientId, string clientSecret, string dataStoreFolderPath, bool shouldDataStoreUseFullPath, uint streamPollingTime, uint chatPollingTime)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            DataStoreFolderPath = dataStoreFolderPath;
            ShouldDataStoreUseFullPath = shouldDataStoreUseFullPath;
            StreamPollingTime = streamPollingTime;
            ChatPollingTime = chatPollingTime;
        }


        /// <summary>
        /// Metoda generující YouTubeOptions z konfigurační sekce
        /// </summary>
        /// <param name="configurationSection">Konfigurační sekce získána z konfiguračního souboru aplikace</param>
        /// <returns>Instanci YouTubeOptions</returns>
        /// <exception cref="ArgumentException">Pokud je v dané konfigurační sekci chyba</exception>
        public static YouTubeOptions CreateFromSection(IConfigurationSection configurationSection)
        {
            string? clientId = configurationSection["ClientId"];
            string? clientSecret = configurationSection["ClientSecret"];
            string? dataStoreFolderPath = configurationSection["DataStoreFolderPath"];
            string? shouldDataStoreUseFullFolderPath = configurationSection["ShouldDataStoreUseFullFolderPath"];
            string? streamPollingTime = configurationSection["StreamPollingTime"];
            string? chatPollingTime = configurationSection["ChatPollingTime"];

            //Kontrola všech hodnot konfigurační sekce
            if (string.IsNullOrEmpty(clientId))
            {
                throw ExceptionHelper.GenerateMissingConfig(nameof(clientId));
            }

            if (string.IsNullOrEmpty(clientSecret))
            {
                throw ExceptionHelper.GenerateMissingConfig(nameof(clientSecret));
            }

            if (string.IsNullOrEmpty(dataStoreFolderPath))
            {
                throw ExceptionHelper.GenerateMissingConfig(nameof(dataStoreFolderPath));
            }

            if (string.IsNullOrEmpty(shouldDataStoreUseFullFolderPath))
            {
                throw ExceptionHelper.GenerateMissingConfig(nameof(shouldDataStoreUseFullFolderPath));
            }

            if (string.IsNullOrEmpty(streamPollingTime))
            {
                throw ExceptionHelper.GenerateMissingConfig(nameof(streamPollingTime));
            }

            if (string.IsNullOrEmpty(chatPollingTime))
            {
                throw ExceptionHelper.GenerateMissingConfig(nameof(chatPollingTime));
            }

            //Kontrola hodnot, které potřebují parsování
            bool validBool = bool.TryParse(shouldDataStoreUseFullFolderPath, out bool shouldDataStoreUseFullFolderPathBool);
            if (!validBool)
            {
                throw new ArgumentException($"The {nameof(ShouldDataStoreUseFullPath)} contains malformed value \"{shouldDataStoreUseFullFolderPath}\"");
            }

            bool validStreamPolling = uint.TryParse(streamPollingTime, out uint streamPollingTimeNumber);
            if (!validStreamPolling)
            {
                throw new ArgumentException($"The {nameof(StreamPollingTime)} contains malformed value \"{streamPollingTime}\"");
            }

            bool validChatPolling = uint.TryParse(chatPollingTime, out uint chatPollingTimeNumber);
            if (!validChatPolling)
            {
                throw new ArgumentException($"The {nameof(ChatPollingTime)} contains malformed value \"{chatPollingTime}\"");
            }

            return new YouTubeOptions(
                clientId,
                clientSecret,
                dataStoreFolderPath,
                shouldDataStoreUseFullFolderPathBool,
                streamPollingTimeNumber,
                chatPollingTimeNumber);
        }
    }
}
