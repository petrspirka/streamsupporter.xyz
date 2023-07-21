namespace NewStreamSupporter.Helpers
{
    /// <summary>
    /// Pomocné metody pro vyvolávání vyjímek
    /// </summary>
    public static class ExceptionHelper
    {
        /// <summary>
        /// Pomocná metoda pro vrácení vyjímky pokud je daný parametr neplatný
        /// </summary>
        /// <param name="parameter">Parametr, který je neplatný</param>
        /// <returns>Instanci třídy ArgumentException</returns>
        public static ArgumentException GenerateMissingConfig(string parameter)
        {
            return new ArgumentException("The IConfigurationSection lacks the " + parameter + " value");
        }
    }
}
