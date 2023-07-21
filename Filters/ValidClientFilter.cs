using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Data;

namespace NewStreamSupporter.Filters
{
    /// <summary>
    /// Filtr obstarávající ochranu DispatcherHubu od neplatných požadavků
    /// </summary>
    public class ValidClientFilter : IHubFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidClientFilter(IServiceProvider provider)
        {
            _serviceProvider = provider;
        }

        //Metoda spoštěna při připojení nového klienta
        public async Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
        {
            //Vytvoříme scpoe pro použití transientních/scoped služeb v singletonech.
            using IServiceScope serviceScope = _serviceProvider.CreateScope();
            ApplicationContext dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationContext>();

            HttpContext? httpContext = context.Context.GetHttpContext();
            if (httpContext == null)
            {
                throw new HubException("Missing HttpContext");
            }
            HttpRequest request = httpContext.Request;

            //klient musí obsahovat všechny potřebné hodnoty
            if (!request.Query.ContainsKey("oid") ||
                !request.Query.ContainsKey("uid") ||
                !request.Query.ContainsKey("type"))
            {
                //Pokud je vyvolána vyjímka HubException, je to bráno jako odmítnutí klienta
                throw new HubException("Invalid request");
            }

            //Kontrola, zda požadavek je pro validní widget (kontrola s databází)
            switch (request.Query["type"])
            {
                case "marquee":
                    bool marquee = await dbContext.Marquees.AnyAsync(m => m.Id == request.Query["oid"].First() && m.Owner.Id == request.Query["uid"].First());
                    if (!marquee)
                    {
                        throw new HubException("Invalid request");
                    }
                    break;
                case "alert":
                    bool alert = await dbContext.Alerts.AnyAsync(m => m.Id == request.Query["oid"].First() && m.Owner.Id == request.Query["uid"].First());
                    if (!alert)
                    {
                        throw new HubException("Invalid request");
                    }
                    break;
                case "counter":
                    bool counter = await dbContext.CounterModel.AnyAsync(m => m.Id == request.Query["oid"].First() && m.Owner.Id == request.Query["uid"].First());
                    if (!counter)
                    {
                        throw new HubException("Invalid request");
                    }
                    break;
                case "donationGoal":
                    bool donationGoal = await dbContext.DonationGoalModel.AnyAsync(m => m.Id == request.Query["oid"].First() && m.Owner.Id == request.Query["uid"].First());
                    if (!donationGoal)
                    {
                        throw new HubException("Invalid request");
                    }
                    break;
                case "timer":
                    bool timer = await dbContext.TimerModel.AnyAsync(m => m.Id == request.Query["oid"].First() && m.Owner.Id == request.Query["uid"].First());
                    if (!timer)
                    {
                        throw new HubException("Invalid request");
                    }
                    break;
                default:
                    throw new HubException("Invalid request");
            };
            //Pokud jsme se dostali až jsem, je požadavek validní, pokračujeme buď do hubu, na který je filtr aplikován, nebo na další filtr.
            await next(context);
        }
    }
}
