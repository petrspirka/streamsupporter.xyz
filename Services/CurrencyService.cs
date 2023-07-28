using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Contracts;
using NewStreamSupporter.Data;
using NewStreamSupporter.Models;
using SQLitePCL;

namespace NewStreamSupporter.Services
{
    /// <summary>
    /// Implementace rozhraní <see cref="ICurrencyService"/>. Slouží pro správu bodů uživatelů
    /// </summary>
    public class CurrencyService : ICurrencyService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        //Mapa platformy -> kanálu -> diváka -> čas. Slouží k rozhodování, zda by daný uživatel měl dostat body
        private readonly IDictionary<Platform, IDictionary<string, IDictionary<string, DateTime>>> _cooldowns;
        private readonly ulong _rewardAmount;
        private readonly ulong _rewardCooldown;
        private readonly ulong _rewardAmountPerDollar;

        //Zámky pro ochranu souběhu
        private readonly object _claimedLock = new();
        private readonly object _unclaimedLock = new();
        private readonly object _cooldownLock = new();

        /// <summary>
        /// Vytvoří novou instanci třídy CurrencyService
        /// </summary>
        /// <param name="serviceProvider">Poskytovatel služeb</param>
        /// <param name="rewardOptions">Nastavení modulu odměn</param>
        public CurrencyService(IServiceProvider serviceProvider, IRewardOptions rewardOptions, ILogger<ICurrencyService> logger)
        {
            _rewardAmount = rewardOptions.RewardAmount;
            _rewardCooldown = rewardOptions.RewardCooldown;
            _rewardAmountPerDollar = rewardOptions.RewardAmountPerDollar;
            _serviceProvider = serviceProvider;
            _logger = logger;

            //Vytvoření základní cooldown mapy a map pro všechny platformy
            _cooldowns = new Dictionary<Platform, IDictionary<string, IDictionary<string, DateTime>>>();

            foreach (object? enumItem in Enum.GetValues(typeof(Platform)))
            {
                _cooldowns[(Platform)enumItem] = new Dictionary<string, IDictionary<string, DateTime>>();
            }
        }


        /// <inheritdoc/>
        public void AddListenerService(BaseListenerService listenerService)
        {
            listenerService.OnStreamDonation += OnListenerServiceStreamDonation;
            listenerService.OnStreamChatMessage += OnListenerServiceChatMessage;
        }

        /// <summary>
        /// Metoda zavolaná, pokud má daná služba typu <see cref="BaseListenerService"/> novou zprávu.
        /// </summary>
        /// <param name="sender">Objekt, který tuto událost vyvolal</param>
        /// <param name="e">Argument události</param>
        private async void OnListenerServiceChatMessage(object? sender, StreamChatMessageEventArgs e)
        {
            string user = e.User.Id;
            using AsyncServiceScope scope = _serviceProvider.CreateAsyncScope();
            ApplicationContext context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            Task addCurrencyTask = Task.CompletedTask;

            //Zamkneme práci s cooldowny, aby se zamezilo duplicitnímu projití zpráv
            lock (_cooldownLock)
            {
                //Pokud neexistuje kanál v mapě, přidáme ho
                if (!_cooldowns[e.User.Platform].ContainsKey(e.Channel))
                {
                    _cooldowns[e.User.Platform][e.Channel] = new Dictionary<string, DateTime>();
                }

                //Pokud uživatel existuje v mapě, zkontrolujeme jeho cooldown, jestli je menší než momentální čas, body mu přičteme
                if (_cooldowns[e.User.Platform][e.Channel]!.ContainsKey(user))
                {
                    if (_cooldowns[e.User.Platform][e.Channel][user] < DateTime.Now)
                    {
                        addCurrencyTask = AddCurrency(e.User.Id, e.Channel, (long)_rewardAmount, e.User.Platform);
                        _cooldowns[e.User.Platform][e.Channel][user] = DateTime.Now.AddMilliseconds(_rewardCooldown);
                    }
                }
                //Pokud uživatel neexistuje, přidáme ho do mapy a přičteme mu body
                else
                {
                    addCurrencyTask = AddCurrency(e.User.Id, e.Channel, (long)_rewardAmount, e.User.Platform);
                    _cooldowns[e.User.Platform][e.Channel][user] = DateTime.Now.AddMilliseconds(_rewardCooldown);
                }
            }
            await addCurrencyTask;
        }


        /// <inheritdoc/>
        public Task<bool> AddCurrency(string userId, string channel, long amount, Platform platform)
        {
            _logger.LogInformation("Adding currency to user: {userId} for channel {channelId}", userId, channel);
            Task<bool> addCurrencyTask = Task.FromResult(true);
            switch (platform)
            {
                case Platform.Twitch:
                    addCurrencyTask = AddTwitchCurrency(userId, channel, amount);
                    break;
                case Platform.YouTube:
                    addCurrencyTask = AddGoogleCurrency(userId, channel, amount);
                    break;
            }
            return addCurrencyTask;
        }

        /// <inheritdoc/>
        public async Task<bool> TakeCurrency(string userId, string channel, long amount)
        {
            ClaimedCurrencyModel? currency = await GetUserCurrency(userId, channel);
            if (currency == null || (long)currency.Currency - amount < 0)
            {
                return false;
            }
            return UpdateExistingUser(userId, channel, -amount);
        }

        /// <summary>
        /// Metoda spuštěná po tom, co kanál dostane příspěvek od uživatele
        /// </summary>
        /// <param name="sender">Objekt, který vyvolal tuto událost</param>
        /// <param name="e">Argument události</param>
        private async void OnListenerServiceStreamDonation(object? sender, StreamDonationEventArgs e)
        {
            ulong amount = Convert.ToUInt64(Math.Round(e.Amount * _rewardAmountPerDollar));
            await AddCurrency(e.User.Id, e.Channel, (long)amount, e.User.Platform);
        }

        /// <inheritdoc/>
        public Task<ClaimedCurrencyModel?> GetUserCurrency(string accountOwnerId, string shopOwnerId)
        {
            using AsyncServiceScope scope = _serviceProvider.CreateAsyncScope();
            ApplicationContext context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            return context.ClaimedCurrencies.Where(c => c.Owner.Id == accountOwnerId && c.ShopOwner.Id == shopOwnerId).FirstOrDefaultAsync();
        }

        /// <inheritdoc/>
        public async Task<ulong> GetUserCurrencyAmount(string accountOwnerId, string shopOwnerId)
        {
            ClaimedCurrencyModel? currency = await GetUserCurrency(accountOwnerId, shopOwnerId);
            ulong amount = currency != null ? currency.Currency : 0UL;
            return amount;
        }

        /// <summary>
        /// Přidání bodů uživateli platformy Twitch
        /// </summary>
        /// <param name="id">Id uživatele</param>
        /// <param name="channel">Id kanálu</param>
        /// <param name="amount">Množství bodů, které přičíst</param>
        /// <returns></returns>
        private async Task<bool> AddTwitchCurrency(string id, string channel, long amount)
        {
            using AsyncServiceScope scope = _serviceProvider.CreateAsyncScope();
            ApplicationContext context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            ApplicationUser? shopOwner = await context.Users.Where(u => u.TwitchId == channel).FirstOrDefaultAsync();
            if(shopOwner == null)
            {
                return false;
            }
            ApplicationUser? user = await context.Users.Where(u => u.TwitchId == id).FirstOrDefaultAsync();
            if (user == null)
            {
                return UpdateMissingUser(id, shopOwner.Id, amount, Platform.Twitch);
            }
            else
            {
                return UpdateExistingUser(user.Id, shopOwner.Id, amount);
            }
        }

        /// <inheritdoc/>
        public bool UpdateMissingUser(string userId, string shopOwnerId, long amount, Platform platform)
        {
            lock (_unclaimedLock)
            {
                using AsyncServiceScope scope = _serviceProvider.CreateAsyncScope();
                ApplicationContext context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                UnclaimedCurrencyModel? unclaimedCurrency = platform switch
                {
                    Platform.Twitch => context.UnclaimedCurrencies.Where(u => u.TwitchId == userId).Where(u => u.OwnerId == shopOwnerId).FirstOrDefault(),
                    Platform.YouTube => context.UnclaimedCurrencies.Where(u => u.GoogleId == userId).Where(u => u.OwnerId == shopOwnerId).FirstOrDefault(),
                    _ => throw new NotImplementedException()
                };

                //Pokud nespárované body neexistují, vytvoříme nový model nespárovaných bodů pro uživatele pokud není množství menší než 0
                if (unclaimedCurrency == null)
                {
                    //Nelze snížit monžství bodů uživateli, který žádné body nemá
                    if (amount < 0)
                    {
                        return false;
                    }
                    unclaimedCurrency = new UnclaimedCurrencyModel
                    {
                        OwnerId = shopOwnerId,
                        Currency = (ulong)amount
                    };
                    switch (platform)
                    {
                        case Platform.Twitch:
                            unclaimedCurrency.TwitchId = userId;
                            break;
                        case Platform.YouTube:
                            unclaimedCurrency.GoogleId = userId;
                            break;
                    }
                    context.UnclaimedCurrencies.Add(unclaimedCurrency);
                }
                else
                {
                    //Kontrola podtečení
                    if ((long)unclaimedCurrency.Currency + amount < 0)
                    {
                        return false;
                    }
                    unclaimedCurrency.Currency += (ulong)amount;
                }

                context.SaveChanges();
            }
            return true;
        }


        /// <inheritdoc/>
        public bool UpdateExistingUser(string userId, string shopOwnerId, long amount)
        {
            lock (_claimedLock)
            {
                using IServiceScope scope = _serviceProvider.CreateScope();
                ApplicationContext context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                ClaimedCurrencyModel? account = context.ClaimedCurrencies.Where(c => c.Owner.Id == userId && c.ShopOwner.Id == shopOwnerId).FirstOrDefault();
                bool exists = true;

                //Pokud účet se spárovanou měnou neexistuje, vytvoříme jej
                if (account == null)
                {
                    exists = false;
                    account = new ClaimedCurrencyModel
                    {
                        OwnerId = userId,
                        ShopOwnerId = shopOwnerId,
                        Currency = 0
                    };
                }

                //Opět nemůžeme brát body pokud uživatel nemá dost bodů
                if ((long)account.Currency + amount < 0)
                {
                    return false;
                }
                account.Currency += (ulong)amount;

                //Pokud měna neexistovala, přidáme ji
                if (!exists)
                {
                    context.ClaimedCurrencies.Add(account);
                }

                context.SaveChangesAsync().GetAwaiter().GetResult();
                return true;
            }
        }

        /// <summary>
        /// Metoda pro modifikaci bodů uživatele platformy YouTube Live
        /// </summary>
        /// <param name="id">Id uživatele v rámci platformy YouTube Live</param>
        /// <param name="channel">Id vlastníka kanánlu v rámci platformy YouTube Live </param>
        /// <param name="amount">Množství bodů, o kolik se má účet uživatele zmšnit</param>
        /// <returns>true, jestli změna proběhla, jinak false</returns>
        private async Task<bool> AddGoogleCurrency(string id, string channel, long amount)
        {
            using AsyncServiceScope scope = _serviceProvider.CreateAsyncScope();
            ApplicationContext context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            ApplicationUser? shopOwner = await context.Users.Where(u => u.GoogleBrandId == channel).FirstOrDefaultAsync();
            if(shopOwner == null)
            {
                return false;
            }
            ApplicationUser? user = await context.Users.Where(u => u.GoogleBrandId == id).FirstOrDefaultAsync();
            if (user == null)
            {
                return UpdateMissingUser(id, shopOwner.Id, amount, Platform.YouTube);
            }
            else
            {
                return UpdateExistingUser(user.Id, shopOwner.Id, amount);
            }
        }

        public async Task Pair(ApplicationUser user)
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            ApplicationContext context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            var twitchId = user.TwitchId;
            var googleId = user.GoogleBrandId;

            var existingTwitchCurrencies = await context.UnclaimedCurrencies.Where(c => c.TwitchId == twitchId).ToListAsync();
            foreach(var currency in existingTwitchCurrencies)
            {
                ClaimedCurrencyModel? claimedCurrency = await context.ClaimedCurrencies.Where(c => c.OwnerId == user.Id).FirstOrDefaultAsync();
                if(claimedCurrency != null)
                {
                    claimedCurrency.Currency += currency.Currency;
                }
                else
                {
                    claimedCurrency = new ClaimedCurrencyModel
                    {
                        Currency = currency.Currency,
                        ShopOwnerId = currency.OwnerId,
                        OwnerId = user.Id
                    };
                    context.ClaimedCurrencies.Add(claimedCurrency);
                    await context.SaveChangesAsync();
                }
                context.UnclaimedCurrencies.Remove(currency);
            }
            await context.SaveChangesAsync();
        
            var existingGoogleCurrencies = await context.UnclaimedCurrencies.Where(c => c.GoogleId == googleId).ToListAsync();
            foreach (var currency in existingGoogleCurrencies)
            {
                ClaimedCurrencyModel? claimedCurrency = await context.ClaimedCurrencies.Where(c => c.OwnerId == user.Id).FirstOrDefaultAsync();
                if (claimedCurrency != null)
                {
                    claimedCurrency.Currency += currency.Currency;
                }
                else
                {
                    claimedCurrency = new ClaimedCurrencyModel
                    {
                        Currency = currency.Currency,
                        ShopOwnerId = currency.OwnerId,
                        OwnerId = user.Id
                    };
                    context.ClaimedCurrencies.Add(claimedCurrency);
                    await context.SaveChangesAsync();
                }
                context.UnclaimedCurrencies.Remove(currency);
            }
            await context.SaveChangesAsync();
        }
    }
}
