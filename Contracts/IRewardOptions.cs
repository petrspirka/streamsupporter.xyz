namespace NewStreamSupporter.Contracts
{
    /// <summary>
    /// Rozhraní představující nastavení modulu odměn
    /// </summary>
    public interface IRewardOptions
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
    }
}