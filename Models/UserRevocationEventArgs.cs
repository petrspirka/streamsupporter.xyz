namespace NewStreamSupporter.Models
{
    /// <summary>
    /// Třída představující argumenty pro událost zrušení autorizace uživatele
    /// </summary>
    public class UserRevocationEventArgs : EventArgs
    {
        /// <summary>
        /// Uživatel, který zrušil autorizaci aplikace
        /// </summary>
        public PlatformUser User { get; set; }

        /// <summary>
        /// Vytvoří novou instanci třídy UserRevocationEventArgs
        /// </summary>
        /// <param name="user">Uživatel, který zrušil autorizaci aplikace</param>
        public UserRevocationEventArgs(PlatformUser user)
        {
            User = user;
        }
    }
}