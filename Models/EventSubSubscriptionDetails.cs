namespace NewStreamSupporter.Models
{
    /// <summary>
    /// Třída sloužící pro reprezentaci detailů odběrů EventSub.
    /// </summary>
    public class EventSubSubscriptionDetails
    {
        /// <summary>
        /// Topic string, který identifikuje druh odběru
        /// </summary>
        public string Topic { get; set; }
        /// <summary>
        /// Dictionary obsahující dvojice klíč - hodnota, které jsou přidány do EventSub požadavku.
        /// </summary>
        public Dictionary<string, string> Condition { get; set; }
        /// <summary>
        /// Verze EventSub odběru. Většinou 1.
        /// </summary>
        public string Version { get; set; }

        public EventSubSubscriptionDetails(string topic, Dictionary<string, string> condition, string version)
        {
            Topic = topic;
            Condition = condition;
            Version = version;
        }
    }
}
