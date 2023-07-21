namespace NewStreamSupporter.Models
{
    /// <summary>
    /// Třída představující argumenty pro událost nové zprávy
    /// </summary>
    public class StreamChatMessageEventArgs : StreamEventArgs
    {
        /// <summary>
        /// Kanál, na kterém byla zpráva poslána
        /// </summary>
        public string Channel { get; }
        /// <summary>
        /// Uživatel, který zprávu poslal
        /// </summary>
        public PlatformUser User { get; }
        /// <summary>
        /// Text zprávy
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Vytvoří novou instanci třídy StreamChatMessageEventArgs
        /// </summary>
        /// <param name="channel">Kanál, na kterém byla zpráva poslána</param>
        /// <param name="user">Uživatel, který zprávu poslal</param>
        /// <param name="message">Text zprávy</param>
        public StreamChatMessageEventArgs(string channel, PlatformUser user, string message) : base(StreamEventType.StreamMessage)
        {
            Channel = channel;
            User = user;
            Message = message;
        }
    }
}
