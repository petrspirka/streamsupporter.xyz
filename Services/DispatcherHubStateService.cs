using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace NewStreamSupporter.Services
{
    public class DispatcherHubStateService
    {
        //Mapa pro převod typu widgetu + id widgetu na všechny připojené widgety
        private readonly IDictionary<string, IDictionary<string, IList<string>>> _connectedClients = new Dictionary<string, IDictionary<string, IList<string>>>();

        private readonly object _lock = new();

        //Zpětná mapa pro převod připojených klientů na typ a Id
        private readonly IDictionary<string, Tuple<string, string>> _reverseConnectedClients = new Dictionary<string, Tuple<string, string>>();
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DispatcherHubStateService> _logger;

        public DispatcherHubStateService(IServiceProvider serviceProvider, ILogger<DispatcherHubStateService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        internal Task ClientConnected(string type, string id, string connectionId)
        {
            lock (_lock)
            {
                //Pokud neexistuje daný typ v mapě, vytvoříme jej
                if (!_connectedClients.ContainsKey(type))
                {
                    _connectedClients[type] = new Dictionary<string, IList<string>>();
                }

                //Mapa id -> seznam připojených widgetů
                IDictionary<string, IList<string>> typedDictionary = _connectedClients[type];

                //Pokud neexistuje Id widgetu v mapě, vytvoříme jej
                if (!typedDictionary.ContainsKey(id))
                {
                    typedDictionary[id] = new List<string>();
                }

                //Přidáme připojeného posluchače do map
                typedDictionary[id]!.Add(connectionId);
                _reverseConnectedClients[connectionId] = new(type, id);
            }

            return Task.CompletedTask;
        }

        internal Task ClientDisconnected(string connectionId)
        {
            lock (_lock)
            {
                //Získáme typ a id widgetu
                Tuple<string, string> reverseLookup = _reverseConnectedClients[connectionId];
                if (reverseLookup == null)
                {
                    return Task.CompletedTask;
                }

                string type = reverseLookup.Item1;
                string id = reverseLookup.Item2;


                //Jestliže existují v mapě připojených tento klient, odstraníme ho
                //Pokud je navíc seznam klientů pro dané id nebo typ prázdný, smažeme ho
                _connectedClients[type]?[id]?.Remove(connectionId);
                if (_connectedClients?[type]?[id]?.Count == 0)
                {
                    _connectedClients[type].Remove(id);
                }

                if (_connectedClients?[type]?.Count == 0)
                {
                    _connectedClients.Remove(type);
                }

                //Odstraníme klienta z reverse mapy
                _reverseConnectedClients.Remove(connectionId);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Metoda pro vyvolání metody Trigger na widgetu
        /// </summary>
        /// <param name="type">Typ widgetu</param>
        /// <param name="id">Id widgetu</param>
        /// <param name="message">Volitelná zpráva jako argument widgetu</param>
        internal Task Trigger(string type, string id, object? message = null)
        {
            return SendMessage(type, id, "Trigger", message);
        }

        /// <summary>
        /// Metoda pro vyvolání metody Reload na widgetu
        /// </summary>
        /// <param name="type">Typ widgetu</param>
        /// <param name="id">Id widgetu</param>
        internal Task Reload(string type, string id)
        {
            return SendMessage(type, id, "Reload", null);
        }

        /// <summary>
        /// Metoda sloužící pro poslání zprávy všem připojeným klientům poslouchajícím daný widget
        /// </summary>
        /// <param name="type">Typ widgetu</param>
        /// <param name="id">Id widgetu</param>
        /// <param name="method">Metoda, která se má na widgetu zavolat</param>
        /// <param name="arg">Volitelný argument, který má být předán widgetu</param>
        /// <returns></returns>
        private async Task SendMessage(string type, string id, string method, object? arg = null)
        {
            IList<string>? clientIds;
            lock (_lock)
            {
                //Jestli nejsou připojení žádní klienti ke zmíněnému widgetu, končíme
                if (!_connectedClients.ContainsKey(type) || !_connectedClients[type]!.ContainsKey(id))
                {
                    return;
                }

                clientIds = _connectedClients[type][id];
            }

            if (clientIds == null)
            {
                return;
            }

            using AsyncServiceScope scope = _serviceProvider.CreateAsyncScope();
            IHubContext<DispatcherHub> hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<DispatcherHub>>();

            string serializedArg = JsonConvert.SerializeObject(arg);

            try
            {
                //Odešleme zprávy všem widgetům v seznamu
                IClientProxy client = hubContext.Clients.Clients(clientIds);
                _logger.LogInformation("Sending event {type}:{method} to clients", type, method);
                await client.SendAsync(method, serializedArg);
            }
            catch (HubException ex)
            {
                _logger.LogInformation("Error while sending notification to clients: {exception}", ex.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
