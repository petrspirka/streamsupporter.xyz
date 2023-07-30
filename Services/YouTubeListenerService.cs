using NewStreamSupporter.Contracts;
using NewStreamSupporter.Models;

namespace NewStreamSupporter.Services
{
    /// <summary>
    /// Implementace <see cref="BaseListenerService"/> pro platformu YouTube Live.
    /// </summary>
    public class YouTubeListenerService : BaseListenerService
    {
        /// <inheritdoc/>
        public override event EventHandler<StreamChatMessageEventArgs>? OnStreamChatMessage;
        /// <inheritdoc/>
        public override event EventHandler<StreamDonationEventArgs>? OnStreamDonation;
        /// <inheritdoc/>
        public override event EventHandler<StreamEndedEventArgs>? OnStreamDown;
        /// <inheritdoc/>
        public override event EventHandler<StreamStartedEventArgs>? OnStreamUp;
        /// <inheritdoc/>
        public override event EventHandler<StreamFollowEventArgs>? OnStreamFollow;

        //Uchovává informace o uživatelech, kteří mají být posloucháni, a o událostech, které mají být vyvolány. Je potřeba mít uchované uživatele a události i tady, protože pollovací služba vyvolává i události, které nechceme.
        private readonly IDictionary<string, IList<StreamEventType>> _userListeners;

        //Reference na pollovací službu
        private readonly IYouTubePollingService _pollingService;

        private readonly ILogger<YouTubeListenerService> _logger;

        /// <summary>
        /// Vytvoří novou instanci třídy YouTubeListenerService
        /// </summary>
        /// <param name="pollingService">Služby pro pollování YouTube API</param>
        public YouTubeListenerService(IYouTubePollingService pollingService, ILogger<YouTubeListenerService> logger)
        {
            _userListeners = new Dictionary<string, IList<StreamEventType>>();
            _pollingService = pollingService;

            //namapování interních eventů pollovací služby na externí eventy
            pollingService.OnStreamChatMessage += (sender, e) => OnStreamChatMessage?.Invoke(sender, e);
            pollingService.OnStreamFollow += (sender, e) => OnStreamFollow?.Invoke(sender, e);
            pollingService.OnStreamDown += (sender, e) => OnStreamDown?.Invoke(sender, e);
            pollingService.OnStreamUp += (sender, e) => OnStreamUp?.Invoke(sender, e);
            pollingService.OnStreamDonation += (sender, e) => OnStreamDonation?.Invoke(sender, e);
            _logger = logger;
        }

        /// <inheritdoc/>
        public override Task<bool> AddUserListener(string userId, StreamEventType eventType)
        {
            _logger.LogInformation("Adding user " + userId);
            if (!_userListeners.ContainsKey(userId))
            {
                _userListeners[userId] = new List<StreamEventType>();
            }
            _userListeners[userId].Add(eventType);
            _pollingService.AddUserListener(userId);
            return Task.FromResult(true);
        }

        /// <inheritdoc/>
        public override Task<bool> RemoveUserListener(string userId, StreamEventType eventType, bool bypassExistenceCheck = false)
        {
            _logger.LogInformation("Removing user " + userId);
            bool success = _userListeners[userId].Remove(eventType);
            if (_userListeners.Count == 0)
            {
                _userListeners.Remove(userId);
                _pollingService.RemoveUserListener(userId);
            }
            return Task.FromResult(success);
        }

        /// <inheritdoc/>
        public override Task InitializeAsync()
        {
            //Není potřebná inicializace
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _pollingService.StartAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _pollingService.StopAsync(cancellationToken);
        }
    }
}
