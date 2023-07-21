using NewStreamSupporter.Contracts;
using NewStreamSupporter.Helpers;
using NewStreamSupporter.Models;
using TwitchLib.EventSub.Webhooks.Core;
using TwitchLib.EventSub.Webhooks.Core.EventArgs.Channel;
using TwitchLib.EventSub.Webhooks.Core.EventArgs.Stream;
using TwitchLib.EventSub.Webhooks.Core.EventArgs.User;

namespace NewStreamSupporter.Services.Twitch
{
    /// <summary>
    /// Implementace rozhraní <see cref="ITwitchEventSubWebhookReceiver"/>
    /// </summary>
    public class TwitchEventSubWebhookReceiver : ITwitchEventSubWebhookReceiver
    {

        /// <inheritdoc/>
        public event EventHandler<StreamDonationEventArgs>? OnStreamDonation;
        /// <inheritdoc/>
        public event EventHandler<StreamEndedEventArgs>? OnStreamDown;
        /// <inheritdoc/>
        public event EventHandler<StreamStartedEventArgs>? OnStreamUp;
        /// <inheritdoc/>
        public event EventHandler<StreamFollowEventArgs>? OnStreamFollow;
        /// <inheritdoc/>
        public event EventHandler<UserRevocationEventArgs>? OnUserRevocation;

        /// <summary>
        /// Vytvoří novou instanci třídy TwitchEventSubWebhookReceiver
        /// </summary>
        /// <param name="webhooks">Interní třída pro získávání notifikací knihovny TwitchLib.Net</param>
        public TwitchEventSubWebhookReceiver(IEventSubWebhooks webhooks)
        {
            webhooks.OnChannelCheer += OnChannelCheer;
            webhooks.OnChannelSubscribe += OnChannelSubscribe;
            webhooks.OnChannelSubscriptionGift += OnChannelSubscriptionGift;
            webhooks.OnChannelSubscriptionMessage += OnChannelSubscriptionMessage;
            webhooks.OnStreamOnline += OnStreamOnline;
            webhooks.OnStreamOffline += OnStreamOffline;
            webhooks.OnChannelFollow += OnChannelFollow;
            webhooks.OnUserAuthorizationRevoke += OnUserAuthorizationRevoke;
        }

        //U všech metod dochází k překladu vnitřních argumentů knihovny TwitchLib.Net a vyvolání událostí aplikace.
        private void OnChannelSubscriptionMessage(object? sender, ChannelSubscriptionMessageArgs e)
        {
            OnStreamDonation?.Invoke(this, new(e.Notification.Event.BroadcasterUserId, new(e.Notification.Event.UserId, e.Notification.Event.UserName, Platform.Twitch), TwitchHelpers.GetSubPlanValue(e.Notification.Event.Tier), e.Notification.Event.Message.Text));
        }

        private void OnStreamOnline(object? sender, StreamOnlineArgs e)
        {
            OnStreamUp?.Invoke(this, new(e.Notification.Event.BroadcasterUserId));
        }


        private void OnStreamOffline(object? sender, StreamOfflineArgs e)
        {
            OnStreamDown?.Invoke(this, new(e.Notification.Event.BroadcasterUserId));
        }

        private void OnUserAuthorizationRevoke(object? sender, UserAuthorizationRevokeArgs e)
        {
            OnUserRevocation?.Invoke(this, new(new(e.Notification.Event.UserId, e.Notification.Event.UserName, Platform.Twitch)));
        }

        private void OnChannelSubscriptionGift(object? sender, ChannelSubscriptionGiftArgs e)
        {
            TwitchLib.EventSub.Core.SubscriptionTypes.Channel.ChannelSubscriptionGift eventData = e.Notification.Event;
            OnStreamDonation?.Invoke(this, new(eventData.BroadcasterUserId, new(eventData.UserId, eventData.UserName, Platform.Twitch), TwitchHelpers.GetSubPlanValue(eventData.Tier) * eventData.Total));
        }

        private void OnChannelFollow(object? sender, ChannelFollowArgs e)
        {
            TwitchLib.EventSub.Core.SubscriptionTypes.Channel.ChannelFollow eventData = e.Notification.Event;
            OnStreamFollow?.Invoke(this, new StreamFollowEventArgs(eventData.BroadcasterUserId, new PlatformUser(eventData.UserId, eventData.UserName, Platform.Twitch)));
        }

        private void OnChannelCheer(object? sender, ChannelCheerArgs e)
        {
            TwitchLib.EventSub.Core.SubscriptionTypes.Channel.ChannelCheer eventData = e.Notification.Event;
            OnStreamDonation?.Invoke(this, new StreamDonationEventArgs(eventData.BroadcasterUserId, new PlatformUser(eventData.UserId, eventData.UserName, Platform.Twitch), eventData.Bits / 100, eventData.Message));
        }

        private void OnChannelSubscribe(object? sender, ChannelSubscribeArgs e)
        {
            TwitchLib.EventSub.Core.SubscriptionTypes.Channel.ChannelSubscribe eventData = e.Notification.Event;
            float amount = TwitchHelpers.GetSubPlanValue(eventData.Tier);
            OnStreamDonation?.Invoke(this, new StreamDonationEventArgs(eventData.BroadcasterUserId, new PlatformUser(eventData.UserId, eventData.UserName, Platform.Twitch), amount));
        }
    }
}