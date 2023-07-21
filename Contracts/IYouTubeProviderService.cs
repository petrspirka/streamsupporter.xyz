using Google.Apis.Auth.OAuth2;
using Google.Apis.YouTube.v3.Data;
using NewStreamSupporter.Models;

namespace NewStreamSupporter.Contracts
{
    /// <summary>
    /// Rozhraní sloužící pro poskytnutí dat z YouTube API
    /// </summary>
    public interface IYouTubeProviderService
    {
        /// <summary>
        /// Získá informace o kanálu uživatele z jeho Access Tokenu
        /// </summary>
        /// <param name="accessToken">Access Token uživatele</param>
        /// <returns>Informace o uživateli</returns>
        public Task<PlatformUser?> GetYoutubeInfoForBrandAccount(string accessToken);
        /// <summary>
        /// Uloží uživatelské tokeny
        /// </summary>
        /// <param name="userId">Uživatelské id v rámci platformy</param>
        /// <param name="refreshTokem">Refresh Token uživatele</param>
        public Task SaveUserCredential(string userId, string refreshTokem);
        /// <summary>
        /// Získá tokeny pro daného uživatele
        /// </summary>
        /// <param name="userId">Uživatelské id v rámci platformy</param>
        /// <returns>Uložené tokeny uživatele</returns>
        public Task<UserCredential> GetUserCredential(string userId);
        /// <summary>
        /// Získá aktivní streamy daného uživatele
        /// </summary>
        /// <param name="userId">Uživatelské id v rámci platformy</param>
        /// <returns>Seznam aktivních streamů</returns>
        public Task<IEnumerable<LiveBroadcast>?> GetActiveStreamsForUser(string userId);
        /// <summary>
        /// Získá zprávy daného uživatele
        /// </summary>
        /// <param name="userId">Uživatelské id v rámci platformy</param>
        /// <param name="liveChatId">Id chatu</param>
        /// <param name="nextPageToken">Volitelný stránkovací token (předchází se jím k opakovaným zprávám</param>
        /// <returns>Odpověď YouTube API se zprávami, nebo null</returns>
        public Task<LiveChatMessageListResponse?> GetChatMessagesForUser(string userId, string liveChatId, string? nextPageToken);
    }
}
