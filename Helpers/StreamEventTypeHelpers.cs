using NewStreamSupporter.Models;

namespace NewStreamSupporter.Helpers
{
    public static class StreamEventTypeHelpers
    {
        //Obsahuje všechny typy pro lazy evaluaci
        private static IReadOnlyCollection<StreamEventType>? _types = null;
        private static readonly object _lock = new();

        //Obsahuje všechny typy enumu StreamEventType
        public static IReadOnlyCollection<StreamEventType> StreamEventTypes
        {
            get
            {
                if (_types == null)
                {
                    //zámek, aby nedošlo k vzniku několika těchto polí
                    lock (_lock)
                    {
                        _types ??= Array.AsReadOnly((StreamEventType[])Enum.GetValues(typeof(StreamEventType)));
                    }
                }
                return _types;
            }
        }

        /// <summary>
        /// Pomocná metoda, která převede daný StreamEventType na řetězec topic používaný interně aplikací Twitch
        /// </summary>
        /// <param name="eventType">Objekt, který se má převést</param>
        /// <returns>Pole všech topic řetězců pro daný StreamEventType</returns>
        /// <exception cref="NotSupportedException">Pokud daný StreamEventType nemá žádné topic řetězce</exception>
        public static string[] GetTwitchEventSubTopics(this StreamEventType eventType)
            => eventType switch
            {
                StreamEventType.StreamFollow => new string[] { "channel.follow" },
                StreamEventType.StreamEnded => new string[] { "stream.offline" },
                StreamEventType.StreamStarted => new string[] { "stream.online" },
                StreamEventType.StreamDonation => new string[] { "channel.subscribe", "channel.subscription.gift", "channel.cheer" },
                StreamEventType.StreamStatusChanged => new string[] { "stream.online", "stream.offline" },
                _ => throw new NotSupportedException($"The specified {nameof(StreamEventType)} {eventType} does not have a valid topic")
            };

        /// <summary>
        /// Pomocná metoda, která k danému StreamEventType vygeneruje detaily o EventSub odběru, který slouží pro registraci handlerů
        /// </summary>
        /// <param name="eventType">StreamEventType, pro který se mají detaily vygenerovat</param>
        /// <param name="userIdentifier">Twitch Id uživatele, pro kterého se mají informace generovat</param>
        /// <returns>Seznam detailů pro EventSub odběry</returns>
        /// <exception cref="NotSupportedException">Pokud daný StreamEventType nemá žádné topic řetězce</exception>
        public static IList<EventSubSubscriptionDetails> GetEventSubSubscriptionsDetails(this StreamEventType eventType, string userIdentifier)
        {
            IList<EventSubSubscriptionDetails> details = new List<EventSubSubscriptionDetails>();
            string[] topics = eventType.GetTwitchEventSubTopics();
            foreach (string topic in topics)
            {
                Dictionary<string, string> condition = new()
                {
                    { "broadcaster_user_id", userIdentifier }
                };
                //StreamFollow dodatečně potřebuje moderator_user_id
                if (eventType == StreamEventType.StreamFollow)
                {
                    condition.Add("moderator_user_id", userIdentifier);
                }
                //StreamFollow navíc pracuje na verzi 2, je tedy potřeba ji nastavit
                details.Add(new(topic, condition, eventType == StreamEventType.StreamFollow ? "2" : "1"));
            }
            return details;
        }
    }
}
