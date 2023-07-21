using NewStreamSupporter.Models;

namespace NewStreamSupporter.Contracts
{
    /// <summary>
    /// Služba sloužící pro pollování YouTube API
    /// </summary>
    public interface IYouTubePollingService : IHostedService
    {
        /// <summary>
        /// Událost nové zprávy
        /// </summary>
        event EventHandler<StreamChatMessageEventArgs>? OnStreamChatMessage;
        /// <summary>
        /// Událost nového příspěvku
        /// </summary>
        event EventHandler<StreamDonationEventArgs>? OnStreamDonation;
        /// <summary>
        /// Událost ukončení streamu
        /// </summary>
        event EventHandler<StreamEndedEventArgs>? OnStreamDown;
        /// <summary>
        /// Událost nového sledujícího
        /// </summary>
        event EventHandler<StreamFollowEventArgs>? OnStreamFollow;
        /// <summary>
        /// Událost začátku streamu
        /// </summary>
        event EventHandler<StreamStartedEventArgs>? OnStreamUp;

        /// <summary>
        /// Přidá nového uživatele, pro kterého se má pollovat
        /// </summary>
        /// <param name="userId">Id uživatele v ránci platformy</param>
        public void AddUserListener(string userId);
        /// <summary>
        /// Odstraní pollovaného uživatele
        /// </summary>
        /// <param name="userId">Id uživatele v rámci platformy</param>
        public void RemoveUserListener(string userId);
    }
}