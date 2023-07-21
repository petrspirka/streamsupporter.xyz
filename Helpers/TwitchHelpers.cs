namespace NewStreamSupporter.Helpers
{
    public static class TwitchHelpers
    {
        /// <summary>
        /// Pomocná metoda převádějící interní "tier" odběratelů na měnu v dolarech 
        /// </summary>
        /// <param name="plan">Řetězec plánu, pro který se má vrátit měna v dolarech</param>
        /// <returns>Počet dolarů, které uživatel přispěl</returns>
        /// <exception cref="ArgumentException">Pokud daný řetězec není platný tier</exception>
        public static float GetSubPlanValue(string plan)
            => plan switch
            {
                "1000" => 4.99f,
                "2000" => 9.99f,
                "3000" => 24.99f,
                _ => throw new ArgumentException("The specified value is not a valid plan string", nameof(plan))
            };
    }
}
