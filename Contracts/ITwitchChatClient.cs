using NewStreamSupporter.Models;

namespace NewStreamSupporter.Contracts
{
    /// <summary>
    /// Rozhraní pro posluchače chatu platformy Twitch
    /// </summary>
    public interface ITwitchChatClient : IHostedService
    {
        /// <summary>
        /// Událost nové zprávy
        /// </summary>
        public event EventHandler<StreamChatMessageEventArgs>? StreamChatMessageReceived;
        /// <summary>
        /// Připojení klienta k danému kanálu
        /// </summary>
        /// <param name="channelId">Id kanálu</param>
        public void JoinChannel(string channelId);
        /// <summary>
        /// Odpojení klienta z daného kanálu
        /// </summary>
        /// <param name="channelId">Id kanálu</param>
        public void LeaveChannel(string channelId);
    }
}