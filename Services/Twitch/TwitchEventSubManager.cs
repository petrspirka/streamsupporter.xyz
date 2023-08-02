using NewStreamSupporter.Contracts;
using TwitchLib.Api.Core.Exceptions;
using TwitchLib.Api.Helix;
using TwitchLib.Api.Helix.Models.EventSub;
using TwitchLib.Api.Interfaces;

namespace NewStreamSupporter.Services.Twitch
{
    /// <summary>
    /// Implementace služby <see cref="ITwitchEventSubManager"/>
    /// </summary>
    public class TwitchEventSubManager : ITwitchEventSubManager
    {
        private readonly Uri _webhookCallback;
        private readonly string _webhookSecret;

        //Uchovává všechny aktivní odběry EventSub
        private IList<EventSubSubscription> _userSubscriptions;
        private bool _initialized = false;
        private readonly EventSub _eventSubApi;
        private readonly ILogger _logger;

        /// <summary>
        /// Vytvoří novou instanci třídy TwitchEventSubManager
        /// </summary>
        /// <param name="twitchAPI">Klient pro komunikaci s Twitch API</param>
        /// <param name="twitchOptions">Nastavení modulu Twitch</param>
        public TwitchEventSubManager(ITwitchAPI twitchAPI, ITwitchOptions twitchOptions, ILogger<TwitchEventSubManager> logger)
        {
            _userSubscriptions = new List<EventSubSubscription>();
            _webhookCallback = twitchOptions.WebhookCallbackUri;
            _webhookSecret = twitchOptions.WebhookSecret;
            _eventSubApi = twitchAPI.Helix.EventSub;
            _logger = logger;
        }


        /// <inheritdoc/>
        public IEnumerable<EventSubSubscription> GetSubscriptionsForUser(string userIdentifier)
        {
            return _userSubscriptions.Where(s => s.Condition["broadcaster_user_id"] == userIdentifier);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteSubscription(string subscriptionId, bool bypassExistingCheck = false)
        {
            if (!_initialized)
            {
                return false;
            }
            EventSubSubscription? subscription = _userSubscriptions.Where(s => s.Id == subscriptionId).FirstOrDefault();

            if (!bypassExistingCheck && subscription == null)
            {
                return false;
            }

            bool succeeded = false;

            //Pokusíme se odstanit zadaný odběr
            try
            {
                succeeded = await _eventSubApi.DeleteEventSubSubscriptionAsync(subscriptionId);
            }
            catch (BadResourceException ex)
            {
                _logger.LogCritical("Something has gone seriously wrong! {message}", ex.Message);
            }
            if (!succeeded)
            {
                return false;
            }

            if (subscription != null)
            {
                _userSubscriptions.Remove(subscription);
            }

            return true;
        }

        private Task<bool> DeleteSubscriptionInternal(string id)
        {
            return _eventSubApi.DeleteEventSubSubscriptionAsync(id);
        }

        /// <summary>
        /// Metoda sloužící pro kontrolu, zda existuje daný EventSub odběr
        /// </summary>
        /// <param name="condition">Podmínka specifikující odběr</param>
        /// <returns>true, pokud odbšr existuje, jinak false</returns>
        private bool SubscriptionExists(Dictionary<string, string> condition, string topic)
        {
            return _userSubscriptions.Any(s => s.Type == topic && condition.Count == s.Condition.Count && condition.All(c => s.Condition.Contains(c)));
        }

        /// <inheritdoc/>
        public async Task<bool> CreateSubscription(string topic, Dictionary<string, string> condition, string version = "1")
        {
            if (!_initialized || SubscriptionExists(condition, topic))
            {
                return true;
            }
            CreateEventSubSubscriptionResponse response;
            try
            {
                //Vytvoření nového EventSub odběru
                response = await _eventSubApi.CreateEventSubSubscriptionAsync(topic, version, condition, TwitchLib.Api.Core.Enums.EventSubTransportMethod.Webhook, null, _webhookCallback.ToString(), _webhookSecret);
            }
            catch (Exception)
            {
                return false;
            }

            EventSubSubscription? subscription = response.Subscriptions.FirstOrDefault();
            if (subscription == null)
            {
                return false;
            }

            //Pokud je cost != 0, pak došlo k chybě 
            if (subscription.Cost != 0)
            {
                if (!await DeleteSubscription(subscription.Id, true))
                {
                    throw new Exception("Failed to delete failed subscription. This should never happen!");
                }
                return false;
            }

            _userSubscriptions.Add(subscription);
            return true;
        }

        /// <inheritdoc/>
        public async Task InitializeAsync()
        {
            _userSubscriptions = new List<EventSubSubscription>();
            GetEventSubSubscriptionsResponse createdSubscriptions = await _eventSubApi.GetEventSubSubscriptionsAsync();
            foreach (EventSubSubscription? subscription in createdSubscriptions.Subscriptions)
            {
                Uri subscriptionUri = new(subscription.Transport.Callback);
                //Odstraníme všechny odběry, které jsou na jiném callbacku, než je daný konfiguračním souborem, nebo pokud odběr má cenu (správně autorizované požadavky mají vždy cenu 0)
                if (subscription.Cost > 0 || Uri.Compare(subscriptionUri, _webhookCallback, UriComponents.AbsoluteUri, UriFormat.UriEscaped, StringComparison.InvariantCulture) != 0)
                {
                    bool success = await DeleteSubscriptionInternal(subscription.Id);
                    if (!success)
                    {
                        throw new Exception("Failed to unsubscribe, this is really bad!");
                    }
                }
                else
                {
                    //Všechny validní odběry přidáme do této mapy
                    _userSubscriptions.Add(subscription);
                }
            }
            _initialized = true;
        }
    }
}
