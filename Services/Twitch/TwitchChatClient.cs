using NewStreamSupporter.Contracts;
using NewStreamSupporter.Models;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;

namespace NewStreamSupporter.Services.Twitch
{
    /// <summary>
    /// Implementace třídy <see cref="ITwitchChatClient"/>
    /// </summary>
    public class TwitchChatClient : ITwitchChatClient
    {
        private readonly ITwitchClient _client;

        //Kanály, které čekají na připojení
        private readonly IList<string> _queuedChannels;

        public event EventHandler<StreamChatMessageEventArgs>? StreamChatMessageReceived;

        private bool _isInitialized = false;

        /// <summary>
        /// Vytvoří novou instanci třídy TwitchChatClient
        /// </summary>
        /// <param name="twitchClient">Služba knihovny TwitchLib.Net sloužící pro připojení do chatu</param>
        public TwitchChatClient(ITwitchClient twitchClient)
        {
            _client = twitchClient;
            _client.OnMessageReceived += OnMessageReceived;
            _client.OnConnected += OnConnected;

            _queuedChannels = new List<string>();
        }

        //Metoda sloužící pro připojení uživatelů při spuštění
        private void OnConnected(object? sender, OnConnectedArgs e)
        {
                foreach (string channel in _queuedChannels)
                {
                    _client.JoinChannel(channel);
                }
                _isInitialized = true;
        }

        //Metoda sloužící pro převedení vnitřních argumentů knihovny na argumenty používané aplikací
        private void OnMessageReceived(object? sender, OnMessageReceivedArgs e)
        {
            TwitchLib.Client.Models.ChatMessage message = e.ChatMessage;
            StreamChatMessageReceived?.Invoke(this, new(message.Channel, new(message.UserId, message.Username, Platform.Twitch), message.Message));
        }

        /// <inheritdoc/>
        public void JoinChannel(string channelId)
        {
            if (!_queuedChannels.Contains(channelId)) { 
                _queuedChannels.Add(channelId);
            }

            if (!_client.IsConnected)
            {
                return;
            }

            if (!IsChannelJoined(channelId))
            {
                _client.JoinChannel(channelId);
            }
        }

        /// <inheritdoc/>
        public void LeaveChannel(string channelId)
        {
            _queuedChannels.Remove(channelId);

            if (IsChannelJoined(channelId))
            {
                _client.LeaveChannel(channelId);
            }
        }

        //Metoda kontrolující, zda je klient k danému kanálu připojen
        private bool IsChannelJoined(string channelId)
            => _client.JoinedChannels.Any(c => c.Channel == channelId);

        /// <inheritdoc/>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _client.Connect();
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _client.Disconnect();
            return Task.CompletedTask;
        }
    }
}
