namespace NewStreamSupporter.Models
{
    /// <summary>
    /// Třída představující argumenty pro událost ukončení streamu
    /// </summary>
    public class StreamEndedEventArgs : StreamEventArgs
    {
        /// <summary>
        /// Kanál, na kterém byl stream ukončen
        /// </summary>
        public string Channel { get; }
        /// <summary>
        /// Vytvoří novou instanci třídy StreamEndedEventArgs
        /// </summary>
        /// <param name="channel">Kanál, na kterém byl stream ukončen</param>
        public StreamEndedEventArgs(string channel) : base(StreamEventType.StreamEnded)
        {
            Channel = channel;
        }
    }
}
