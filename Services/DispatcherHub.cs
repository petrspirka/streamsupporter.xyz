using Microsoft.AspNetCore.SignalR;

namespace NewStreamSupporter.Services
{
    public class DispatcherHub : Hub
    {
        private readonly ILogger<DispatcherHub> _logger;
        private readonly DispatcherHubStateService _hubService;

        public DispatcherHub(DispatcherHubStateService hubService, ILogger<DispatcherHub> logger)
        {
            _logger = logger;
            _hubService = hubService;
        }

        /// <summary>
        /// Metoda zavolána po připojení widgetu do hubu.
        /// 
        /// Metoda jej zapíše do map pro pozdější práci s nimi
        /// </summary>
        public override Task OnConnectedAsync()
        {
            //Získání potřebných údajů z požadavku
            HttpRequest request = Context.GetHttpContext()!.Request;
            string id = request.Query["oid"].First()!;
            string type = request.Query["type"].First()!;
            _logger.LogInformation("Client with Id {ConnectionId} connected.", Context.ConnectionId);

            return _hubService.ClientConnected(type, id, Context.ConnectionId);
        }


        /// <summary>
        /// Metoda sloužící pro vyčištění map po odpojení klienta
        /// </summary>
        /// <param name="exception">Volitelný důvod odpojení klienta</param>
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            string connectionId = Context.ConnectionId;
            _logger.LogInformation("Client with Id {ConnectionId} disconnected.", Context.ConnectionId);
            return _hubService.ClientDisconnected(connectionId);
        }
    }
}
