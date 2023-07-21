using NewStreamSupporter.Models;

namespace NewStreamSupporter.Contracts
{
    /// <summary>
    /// Rozhraní sloužící pro získávání EventSub notifikací
    /// </summary>
    public interface ITwitchEventSubWebhookReceiver
    {
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
        /// Událost zrušení autorizace uživatelem
        /// </summary>
        event EventHandler<UserRevocationEventArgs>? OnUserRevocation;
    }
}