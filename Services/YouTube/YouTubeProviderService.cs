using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using NewStreamSupporter.Contracts;
using NewStreamSupporter.Models;

namespace NewStreamSupporter.Services.YouTube
{
    /// <summary>
    /// Implementace třídy <see cref="IYouTubeProviderService"/>
    /// </summary>
    public class YouTubeProviderService : IYouTubeProviderService
    {
        private readonly YouTubeService _service;
        private readonly IServiceProvider _serviceProvider;

        //Použito ke kontrole překročení kvóty
        private bool _quotaReached = false;

        //Obsahuje informace o klientovi
        private readonly ClientSecrets _clientSecrets;

        /// <summary>
        /// Vytvoří novou instanci třídy YouTubeProviderService
        /// </summary>
        /// <param name="options">Nastavení modulu YouTube Live</param>
        /// <param name="serviceProvider">Poskytovatel služeb</param>
        /// <param name="service">Služba sloužící pro komunikaci s YouTube API</param>
        public YouTubeProviderService(IYouTubeOptions options, IServiceProvider serviceProvider, YouTubeService service)
        {
            _serviceProvider = serviceProvider;
            _service = service;
            _clientSecrets = new ClientSecrets()
            {
                ClientId = options.ClientId,
                ClientSecret = options.ClientSecret
            };
        }

        /// <inheritdoc/>
        public async Task<PlatformUser?> GetYoutubeInfoForBrandAccount(string accessToken)
        {
            if (_quotaReached)
            {
                throw CreateQuotaException();
            }
            //Specifikace požadavku
            ChannelsResource.ListRequest request = _service.Channels.List("snippet");
            request.AccessToken = accessToken;
            request.MaxResults = 1;
            request.Mine = true;
            ChannelListResponse result;
            try
            {
                result = await request.ExecuteAsync();
            }
            catch (Exception ex)
            {
                //Překročení kvóty
                if (ex.Message.Contains("quota", StringComparison.InvariantCultureIgnoreCase))
                {
                    _quotaReached = true;
                    throw CreateQuotaException();
                }
                throw;
            }
            if (result!.Items == null || result!.Items.Count <= 0)
            {
                return null;
            }

            Channel user = result.Items[0];

            return new PlatformUser(user.Id, user.Snippet.Title, Platform.YouTube);
        }

        /// <inheritdoc/>
        public async Task SaveUserCredential(string userId, string refreshToken)
        {
            using AsyncServiceScope scope = _serviceProvider.CreateAsyncScope();
            IDataStore dataStore = scope.ServiceProvider.GetRequiredService<IDataStore>();
            GoogleAuthorizationCodeFlow flow = new(new GoogleAuthorizationCodeFlow.Initializer()
            {
                ClientSecrets = _clientSecrets,
                DataStore = dataStore
            });
            UserCredential userCredentials = new(flow, userId, new TokenResponse()
            {
                RefreshToken = refreshToken
            });
            //Donutí IDataStore uložit data
            await userCredentials.RefreshTokenAsync(CancellationToken.None);
        }

        /// <inheritdoc/>
        public async Task<UserCredential> GetUserCredential(string userId)
        {
            using AsyncServiceScope scope = _serviceProvider.CreateAsyncScope();
            IDataStore dataStore = scope.ServiceProvider.GetRequiredService<IDataStore>();
            GoogleAuthorizationCodeFlow flow = new(new GoogleAuthorizationCodeFlow.Initializer()
            {
                ClientSecrets = _clientSecrets,
                DataStore = dataStore
            });
            UserCredential userCredentials = new(flow, userId, await dataStore.GetAsync<TokenResponse>(userId));
            return userCredentials;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LiveBroadcast>?> GetActiveStreamsForUser(string userId)
        {
            if (_quotaReached)
            {
                throw CreateQuotaException();
            }
            //Specifikace požadavku
            LiveBroadcastsResource.ListRequest request = _service.LiveBroadcasts.List("snippet");
            request.AddCredential(await GetUserCredential(userId));
            request.BroadcastStatus = LiveBroadcastsResource.ListRequest.BroadcastStatusEnum.Active;
            LiveBroadcastListResponse result;
            try
            {
                result = await request.ExecuteAsync();
            }
            catch (Exception ex)
            {
                //Překročení kvóty
                if (ex.Message.Contains("quota", StringComparison.InvariantCultureIgnoreCase))
                {
                    _quotaReached = true;
                    throw CreateQuotaException();
                }
                if (ex.Message.Contains("invalid_grant", StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new UnauthorizedAccessException(ex.Message, ex);
                }
                return null;
            }
            return result.Items;
        }

        /// <inheritdoc/>
        public async Task<LiveChatMessageListResponse?> GetChatMessagesForUser(string userId, string liveChatId, string? nextPageToken)
        {
            if (_quotaReached)
            {
                throw CreateQuotaException();
            }
            //Specifikace požadavku
            LiveChatMessagesResource.ListRequest request = _service.LiveChatMessages.List(liveChatId, new Google.Apis.Util.Repeatable<string>(new string[] { "snippet", "authorDetails" }));
            request.AddCredential(await GetUserCredential(userId));
            request.PageToken = nextPageToken;
            request.MaxResults = int.MaxValue;
            LiveChatMessageListResponse result;
            try
            {
                result = await request.ExecuteAsync();
            }
            catch (Exception ex)
            {
                //Překročení kvóty
                if (ex.Message.Contains("quota", StringComparison.InvariantCultureIgnoreCase))
                {
                    _quotaReached = true;
                    throw CreateQuotaException();
                }
                if (ex.Message.Contains("invalid_grant", StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new UnauthorizedAccessException(ex.Message, ex);
                }
                return null;
            }
            return result;
        }


        private static Exception CreateQuotaException()
        {
            return new Exception("Quota limit reached");
        }
    }
}
