namespace NewStreamSupporter.Contracts
{
    /// <summary>
    /// Rozhraní představující konfigure YouTube Live modulu
    /// </summary>
    public interface IYouTubeOptions
    {
        /// <summary>
        /// Klientské Id aplikace
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// Klíč klienta aplikace
        /// </summary>
        public string ClientSecret { get; set; }
        /// <summary>
        /// Cesta k úložišti YouTube tokenů. Pokud není parametr <see cref="ShouldDataStoreUseFullPath"/> false, cesta je relativní vůči adresáři AppData aplikace
        /// </summary>
        public string DataStoreFolderPath { get; set; }
        /// <summary>
        /// Zda je DataStoreFolderPath cesta absolutní nebo relativní vůči složce aplikace v AppData 
        /// </summary>
        public bool ShouldDataStoreUseFullPath { get; set; }
        /// <summary>
        /// Interval pollování endpointu pro Streamy
        /// </summary>
        public uint StreamPollingTime { get; set; }
        /// <summary>
        /// Interval pollování endpointu pro Chat streamu
        /// </summary>
        public uint ChatPollingTime { get; set; }
    }
}