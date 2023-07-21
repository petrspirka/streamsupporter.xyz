using NewStreamSupporter.Helpers;
using NewStreamSupporter.Models;

namespace NewStreamSupporter.Contracts
{
    /// <summary>
    /// Základní třída pro všechny posluchače
    /// </summary>
    public abstract class BaseListenerService : IHostedService
    {
        /// <summary>
        /// Událost nové zprávy
        /// </summary>
        public abstract event EventHandler<StreamChatMessageEventArgs>? OnStreamChatMessage;
        /// <summary>
        /// Událost nového příspěvku
        /// </summary>
        public abstract event EventHandler<StreamDonationEventArgs>? OnStreamDonation;
        /// <summary>
        /// Událost ukončení streamu
        /// </summary>
        public abstract event EventHandler<StreamEndedEventArgs>? OnStreamDown;
        /// <summary>
        /// Událost spuštění streamu
        /// </summary>
        public abstract event EventHandler<StreamStartedEventArgs>? OnStreamUp;
        /// <summary>
        /// Událost nového sledujícího
        /// </summary>
        public abstract event EventHandler<StreamFollowEventArgs>? OnStreamFollow;

        /// <summary>
        /// Přidá nového uživatele do této služby
        /// </summary>
        /// <param name="userId">Id uživatele na dané platformě</param>
        /// <param name="eventType">Typ událostí, které mají být poslouchány</param>
        /// <returns>true, pokud byl uživatel úspěšně přidán, jinak false</returns>
        public abstract Task<bool> AddUserListener(string userId, StreamEventType eventType);
        /// <summary>
        /// Odstraní uživatele z této služby
        /// </summary>
        /// <param name="userId">Id uživatele na dané platformě</param>
        /// <param name="eventType">Typ událostí, které mají být poslouchány</param>
        /// <param name="bypassExistenceCheck"></param>
        /// <returns>true, pokud byl uživatel odstraněn, jinak false</returns>
        public abstract Task<bool> RemoveUserListener(string userId, StreamEventType eventType, bool bypassExistenceCheck = false);
        /// <summary>
        /// Inicializuje službu
        /// </summary>
        public abstract Task InitializeAsync();

        /// <inheritdoc/>
        public abstract Task StartAsync(CancellationToken cancellationToken);
        /// <inheritdoc/>
        public abstract Task StopAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Pomocná metoda pro registraci všech událostí danému uživateli
        /// </summary>
        /// <param name="userId">Uživatelské Id v rámci platformy</param>
        /// <returns>Událost, při které selhala registrace</returns>
        /// <exception cref="Exception">Pokud dojde k vnitřnímu selhání aplikace (nelze odebrat právě přidaného uživatele</exception>
        public async Task<StreamEventType?> AddAllUserListeners(string userId)
        {
            IReadOnlyCollection<StreamEventType> types = StreamEventTypeHelpers.StreamEventTypes;
            for (int i = 0; i < types.Count; i++)
            {
                StreamEventType type = types.ElementAt(i);
                bool result = await AddUserListener(userId, type);
                if (!result)
                {
                    for (int j = 0; j < i; j++)
                    {
                        StreamEventType removedType = types.ElementAt(j);
                        bool removalResult = await RemoveUserListener(userId, removedType);
                        if (!removalResult)
                        {
                            throw new Exception("Failed to remove listener");
                        }
                    }
                    return type;
                }
            }
            return null;
        }

        /// <summary>
        /// Pomocná metoda pro odebrání všech událostí danému uživateli
        /// </summary>
        /// <param name="userId">Uživatelské Id v rámci platformy</param>
        /// <returns>Pole událostí, které nebylo možné odstranit</returns>
        public async Task<StreamEventType[]> RemoveAllUserListeners(string userId)
        {
            List<StreamEventType> invalidTypes = new();
            IReadOnlyCollection<StreamEventType> types = StreamEventTypeHelpers.StreamEventTypes;
            for (int i = 0; i < types.Count; i++)
            {
                StreamEventType type = types.ElementAt(i);
                bool result = await RemoveUserListener(userId, type, true);
                if (!result)
                {
                    invalidTypes.Add(type);
                }
            }
            return invalidTypes.ToArray();
        }
    }
}
