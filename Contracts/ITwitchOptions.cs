namespace NewStreamSupporter.Contracts
{
    /// <summary>
    /// Rozhraní představující konfiguraci modulu pro platformu Twitch
    /// </summary>
    public interface ITwitchOptions
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
    }
}
