namespace NewStreamSupporter.Models
{
    /// <summary>
    /// Třída představující argumenty pro událost nového sledujícího streamera
    /// </summary>
    public class StreamFollowEventArgs : StreamEventArgs
    {
        /// <summary>
        /// Kanál, který získal nového sledujícího
        /// </summary>
        public string Channel { get; }
        /// <summary>
        /// Uživatel, který nyní sleduje kanál
        /// </summary>
        public PlatformUser User { get; }
        /// <summary>
        /// Vytvoří novou instanci třídy StreamFollowEventArgs
        /// </summary>
        /// <param name="channel">Kanál, který získal nového sledujícího</param>
        /// <param name="user">Uživatel, který nyní sleduje kanál</param>
        public StreamFollowEventArgs(string channel, PlatformUser user) : base(StreamEventType.StreamFollow)
        {
            Channel = channel;
            User = user;
        }
    }
}
