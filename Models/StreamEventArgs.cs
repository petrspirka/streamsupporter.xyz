namespace NewStreamSupporter.Models
{
    /// <summary>
    /// Základní třída všech jiných argumentů používaných aplikací
    /// </summary>
    public class StreamEventArgs : EventArgs
    {
        /// <summary>
        /// Typ události, který nastal
        /// </summary>
        public StreamEventType EventType { get; }

        /// <summary>
        /// Vytvoří novou instanci třídy StreamEventArgs
        /// </summary>
        /// <param name="type">Typ události, který nastal</param>
        public StreamEventArgs(StreamEventType type) : base()
        {
            EventType = type;
        }
    }
}
