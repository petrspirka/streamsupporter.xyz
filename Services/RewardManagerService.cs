using NewStreamSupporter.Contracts;
using NewStreamSupporter.Data;

namespace NewStreamSupporter.Services
{
    /// <summary>
    /// Služba obstarávající kupování odměn
    /// </summary>
    public class RewardManagerService
    {
        private readonly ApplicationContext _context;
        private readonly ICurrencyService _currencyService;
        private readonly DispatcherHubStateService _hubService;

        /// <summary>
        /// Vytvoří novou instanci třídy RewardManagerService
        /// </summary>
        /// <param name="context">Kontext pro komunikaci s databází</param>
        /// <param name="currencyService">Služba pro sprvování měn</param>
        /// <param name="hub">Instance DispatcherHub sloužící pro notifikaci widgetů aplikace</param>
        public RewardManagerService(ApplicationContext context, ICurrencyService currencyService, DispatcherHubStateService hubService)
        {
            _context = context;
            _currencyService = currencyService;
            _hubService = hubService;
        }

        /// <summary>
        /// Metoda sloužící pro koupení dané odměny daným uživatelem.
        /// </summary>
        /// <param name="userId">Uživatelské Id kupce</param>
        /// <param name="rewardId">Id odměny</param>
        /// <param name="message">Volitelná zprává kupce</param>
        /// <returns>true, jestli byla odměna úspěšně koupena, false pokud ne</returns>
        /// <exception cref="ArgumentException">Pokud odměna neexistuje</exception>
        public async Task<bool> RedeemReward(string userId, string rewardId, string? message = null)
        {
            RewardModel reward = await _context.Rewards.FindAsync(rewardId) ?? throw new ArgumentException("The specified reward does not exist");
            ApplicationUser user = (await _context.Users.FindAsync(userId))!;
            string rewardOwnerId = reward.OwnerId;
            ulong userCurrency = await _currencyService.GetUserCurrencyAmount(userId, rewardOwnerId);

            //Pokud uživatel nemá dostatečné množství bodu, vracíme false
            if (userCurrency < reward.Cost)
            {
                return false;
            }

            long cost = (long)reward.Cost;

            //Pokusíme se odebrat uživateli zmíněný počet bodů
            bool valid = await _currencyService.TakeCurrency(userId, reward.OwnerId, cost);
            if (!valid)
            {
                return false;
            }

            PurchaseModel purchase = new()
            {
                BuyerId = userId,
                RewardId = reward.Id,
                OwnerId = reward.OwnerId,
                CostAtPurchase = (ulong)cost,
                Text = $"{user.UserName} purchased {reward.Name}"
            };

            if (message != null)
            {
                purchase.Text += $"\nMessage: {message}";
            }
            else if (reward.TriggeredType == "alert")
            {
                message = purchase.Text;
            }

            //Pokud je odměna sebe-akceptující, pokusíme se ji akceptovat.
            if (reward.AutoAccept)
            {
                purchase.Confirmed = true;
                purchase.Finished = true;
                if (reward.TriggeredId != null && reward.TriggeredType != null)
                {
                    await TriggerReward(reward.TriggeredType, reward.TriggeredId, message);
                }
            }
            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Pomocná metoda na spuštění odměny
        /// </summary>
        /// <param name="type">Typ cílového widgetu</param>
        /// <param name="id">Id cílového widgetu</param>
        /// <param name="message">Volitelná zpráva uživatele</param>
        /// <returns></returns>
        private Task TriggerReward(string type, string id, string? message)
        {
            return _hubService.Trigger(type, id, message);
        }
    }
}
