namespace NewStreamSupporter.Models
{
    /// <summary>
    /// Třída představující argumenty pro událost přispění peněžní částky
    /// </summary>
    public class StreamDonationEventArgs : StreamEventArgs
    {
        /// <summary>
        /// Kanál, kterému bylo přispěno
        /// </summary>
        public string Channel { get; }
        /// <summary>
        /// Množství v dolarech, které uživatel přispěl
        /// </summary>
        public float Amount { get; }
        /// <summary>
        /// Uživatel, který přispěl
        /// </summary>
        public PlatformUser User { get; }
        /// <summary>
        /// Volitelná zpráva od uživatele
        /// </summary>
        public string? Message { get; }

        /// <summary>
        /// Vytvoří novou instanci třídy StreamDonationEventArgs
        /// </summary>
        /// <param name="channel">Kanál, kterému bylo přispěno</param>
        /// <param name="user">Uživatel, který přispěl</param>
        /// <param name="amount">Množství v dolarech, které uživatel přispěl</param>
        /// <param name="message">Volitelná zpráva uživatele</param>
        public StreamDonationEventArgs(string channel, PlatformUser user, float amount, string? message = null) : base(StreamEventType.StreamDonation)
        {
            Channel = channel;
            Amount = amount;
            User = user;
            Message = message;
        }
    }
}
