using NewStreamSupporter.Contracts;

namespace NewStreamSupporter.Models
{
    /// <summary>
    /// Model sloužící pro namapování nastavení odměn z konfiguračního souboru aplikace.
    /// </summary>
    public class RewardOptions : IRewardOptions
    {
        /// <summary>
        /// Počet bodů za každou zprávu mimo cooldown
        /// </summary>
        public ulong RewardAmount { get; set; }
        /// <summary>
        /// Cooldown v milisekundách, který rozhoduje, jak dlouho musí uživatel čekat, než může znovu dostat body za zprávu
        /// </summary>
        public ulong RewardCooldown { get; set; }
        /// <summary>
        /// Počet bodů, které uživatel dostane za každý dolar, který přispěje
        /// </summary>
        public ulong RewardAmountPerDollar { get; set; }

        /// <summary>
        /// Vytvoří novou instanci třídy RewardOptions. Je doporučeno generovat tuto třídu z konfiguračního souboru použitím metody <see cref="CreateFromSection(IConfigurationSection)"
        /// </summary>
        /// <param name="rewardAmount"></param>
        /// <param name="rewardCooldown"></param>
        /// <param name="rewardAmountPerDollar"></param>
        public RewardOptions(ulong rewardAmount, ulong rewardCooldown, ulong rewardAmountPerDollar)
        {
            RewardAmount = rewardAmount;
            RewardCooldown = rewardCooldown;
            RewardAmountPerDollar = rewardAmountPerDollar;
        }

        /// <summary>
        /// Metoda generující RewardOptions z konfigurační sekce
        /// </summary>
        /// <param name="configurationSection">Konfigurační sekce získána z konfiguračního souboru aplikace</param>
        /// <returns>Instanci RewardOptions</returns>
        /// <exception cref="ArgumentException">Pokud je v dané konfigurační sekci chyba</exception>
        public static RewardOptions CreateFromSection(IConfigurationSection configurationSection)
        {
            string? rewardAmount = configurationSection["RewardAmount"];
            string? rewardCooldown = configurationSection["RewardCooldown"];
            string? rewardAmountPerDollar = configurationSection["RewardAmountPerDollar"];


            return new RewardOptions(
                ParseNumber(rewardAmount),
                ParseNumber(rewardCooldown),
                ParseNumber(rewardAmountPerDollar));
        }

        /// <summary>
        /// Metoda se pokusí převést daný řetězec na číslo
        /// </summary>
        /// <param name="parsedNumber">String, který má být převeden</param>
        /// <returns>Číslo reprezentováno jako ulong</returns>
        /// <exception cref="ArgumentException">Pokud se nepodařilo převést daný řetězec na číslo</exception>
        private static ulong ParseNumber(string? parsedNumber)
        {
            bool valid = ulong.TryParse(parsedNumber, out ulong parsedNumberLong);
            if (!valid)
            {
                throw new ArgumentException("One or more values in the reward module configuration are invalid");
            }
            return parsedNumberLong;
        }
    }
}