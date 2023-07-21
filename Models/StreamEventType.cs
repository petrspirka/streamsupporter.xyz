namespace NewStreamSupporter.Models
{
    /// <summary>
    /// Enumerace všech událostí, které mohou nastat
    /// </summary>
    public enum StreamEventType
    {
        StreamFollow,
        StreamMessage,
        StreamDonation,
        StreamStarted,
        StreamEnded,
        StreamStatusChanged
    }
}
