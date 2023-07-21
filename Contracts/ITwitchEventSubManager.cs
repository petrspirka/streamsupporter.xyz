using TwitchLib.Api.Helix.Models.EventSub;

namespace NewStreamSupporter.Contracts
{
    /// <summary>
    /// Rozhraní sloužící pro správu EventSub odběrů
    /// </summary>
    public interface ITwitchEventSubManager
    {
        /// <summary>
        /// Vytvoří nový EventSub odběr
        /// </summary>
        /// <param name="topic">Topic odběru</param>
        /// <param name="condition">Podmínky odběru</param>
        /// <param name="version">Verze odběru</param>
        /// <returns>true, pokud byl EventSub odběr zrušen, jinak false</returns>
        Task<bool> CreateSubscription(string topic, Dictionary<string, string> condition, string version = "1");
        /// <summary>
        /// Smaže daný EventSub odběr
        /// </summary>
        /// <param name="subscriptionId">Id odběru</param>
        /// <param name="bypassExistingCheck">Říká, zda se má obejít kontrola existence</param>
        /// <returns>true, pokud byl EventSub odběr vytvořen, jinak false</returns>
        Task<bool> DeleteSubscription(string subscriptionId, bool bypassExistingCheck = false);
        /// <summary>
        /// Získá všechny odběry daného uživatele
        /// </summary>
        /// <param name="userIdentifier">Uživatelské Id v rámci platformy</param>
        /// <returns></returns>
        IEnumerable<EventSubSubscription> GetSubscriptionsForUser(string userIdentifier);
        /// <summary>
        /// Inicializace služby
        /// </summary>
        Task InitializeAsync();
    }
}