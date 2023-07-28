using NewStreamSupporter.Data;
using NewStreamSupporter.Models;

namespace NewStreamSupporter.Contracts
{
    /// <summary>
    /// Rozhraní pro práci s body
    /// </summary>
    public interface ICurrencyService
    {
        /// <summary>
        /// Přidá danému uživateli body
        /// </summary>
        /// <param name="userId">Uživatelské id v rámci platformy</param>
        /// <param name="channel">Id kanálu v rámci platformy</param>
        /// <param name="amount">Množství které přidat</param>
        /// <param name="platform">Platforma, na které se uživatelé nachází</param>
        /// <returns></returns>
        Task<bool> AddCurrency(string userId, string channel, long amount, Platform platform);
        /// <summary>
        /// Přidá služby typu <see cref="BaseListenerService"/> do databáze za účelem přidávání bodů za zprávy
        /// </summary>
        /// <param name="listenerService">Služba pro kterou se mají přidat posluchače</param>
        void AddListenerService(BaseListenerService listenerService);
        /// <summary>
        /// Získá model <see cref="ClaimedCurrencyModel"/> reprezentující účet uživatele v daném kanálu
        /// </summary>
        /// <param name="accountOwnerId">Id uživatele</param>
        /// <param name="shopOwnerId">Id kanálu</param>
        /// <returns></returns>
        Task<ClaimedCurrencyModel?> GetUserCurrency(string accountOwnerId, string shopOwnerId);
        /// <summary>
        /// Získá počet bodů daného uživatele
        /// </summary>
        /// <param name="accountOwnerId">Id uživatele</param>
        /// <param name="shopOwnerId">Id kanálu</param>
        /// <returns></returns>
        Task<ulong> GetUserCurrencyAmount(string accountOwnerId, string shopOwnerId);
        /// <summary>
        /// Odebere danému uživateli daný počet bodů
        /// </summary>
        /// <param name="userId">Id uživatele</param>
        /// <param name="channel">Id kanálu</param>
        /// <param name="amount">Počet bodů</param>
        /// <returns></returns>
        Task<bool> TakeCurrency(string userId, string channel, long amount);
        /// <summary>
        /// Metoda pro přímou modifikaci spárovaného uživatele
        /// </summary>
        /// <param name="userId">Uživatelské Id v rámci aplikace</param>
        /// <param name="shopOwnerId">Id vlastníka kanálu</param>
        /// <param name="amount">O kolik bodů měníme uživatele</param>
        /// <returns>true, pokud byly změny úspěšně provedeny, jinak false</returns>
        internal bool UpdateExistingUser(string userId, string shopOwnerId, long amount);

        /// <summary>
        /// Metoda pro přímou modifikaci bodů nespárovaného uživatele
        /// </summary>
        /// <param name="userId">Id uživatele v rámci platformy</param>
        /// <param name="shopOwnerId">Id kanálu</param>
        /// <param name="amount">Množství bodů</param>
        /// <param name="platform">Platforma, ze které notifikace pochází</param>
        /// <returns>true, jestli byly body změněny, jinak false</returns>
        /// <exception cref="NotImplementedException">Pokud je použita nepodporovaná platforma</exception>
        internal bool UpdateMissingUser(string userId, string shopOwnerId, long amount, Platform platform);

        /// <summary>
        /// Metoda pro spárování nespárovaných měn s uživatelem
        /// </summary>
        /// <param name="user">Uživatel, pro kterého se má služba pokusit spároval volné měny</param>
        internal Task Pair(ApplicationUser user);
    }
}