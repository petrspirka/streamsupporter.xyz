namespace NewStreamSupporter.Models
{
    /// <summary>
    /// Třída představující argumenty pro událost začátku streamu 
    /// </summary>
    public class StreamStartedEventArgs : StreamEventArgs
    {
        /// <summary>
        /// Kanál, na kterém stream začal
        /// </summary>
        public string Channel { get; }

        /// <summary>
        /// Vytvoří novou instanci třídy StreamStartedEventArgs
        /// </summary>
        /// <param name="channel">Kanál, na kterém stream začal</param>
        public StreamStartedEventArgs(string channel) : base(StreamEventType.StreamStarted)
        {
            Channel = channel;
        }
    }
}
