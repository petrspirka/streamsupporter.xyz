using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Contracts;
using NewStreamSupporter.Data;
using NewStreamSupporter.Helpers;
using NewStreamSupporter.Services;
using System.Web;

namespace NewStreamSupporter.Areas.Dashboard.Pages
{
    public class IndexModel : PageModel
    {
        public ICollection<NotificationModel>? Notifications { get; set; } = default!;
        public ICollection<PurchaseModel>? Purchases { get; set; } = default!;
        private readonly ApplicationContext _context;
        private readonly ICurrencyService _currencyService;
        private readonly DispatcherHubStateService _hubService;

        public IndexModel(ApplicationContext context, ICurrencyService currencyService, DispatcherHubStateService hubService)
        {
            _context = context;
            _currencyService = currencyService;
            _hubService = hubService;
        }

        public async void OnGet()
        {
            ApplicationUser? user = await _context.Users
                .Where(u => u.Id == HttpContext.GetUserId())
                .Include(u => u.Notifications)
                .Include(u => u.Purchases)
                .FirstOrDefaultAsync();
            Notifications = user!.Notifications ?? new List<NotificationModel>();
            Purchases = (user.Purchases ?? new List<PurchaseModel>()).Where(p => !p.Finished).ToList();
        }

        public async void OnPostAsync(string id, string type, bool? isConfirmed)
        {
            switch (type)
            {
                case nameof(PurchaseModel):
                    await HandlePurchaseModel(id, (bool)isConfirmed!);
                    break;

                case nameof(NotificationModel):
                    await HandleNotificationModel(id);
                    break;
            }
            OnGet();
        }

        private async Task HandleNotificationModel(string id)
        {
            NotificationModel? notification = await _context.Notifications.Where(n => n.Id == id && n.OwnerId == HttpContext.GetUserId()).FirstOrDefaultAsync();
            if (notification != null)
            {
                _context.Remove(notification);
            }
            await _context.SaveChangesAsync();
        }

        private async Task HandlePurchaseModel(string id, bool isConfirmed)
        {
            PurchaseModel purchase = await _context.Purchases.Include(p => p.Reward).Where(p => p.Id == id).FirstAsync();
            if (HttpContext.GetUserId() != purchase!.OwnerId || purchase.Finished)
            {
                return;
            }

            purchase.Confirmed = isConfirmed;
            purchase.Finished = true;

            if (!isConfirmed)
            {
                _currencyService.UpdateExistingUser(purchase.BuyerId, purchase.OwnerId, (long)purchase.CostAtPurchase);
                await _context.SaveChangesAsync();
            }
            else if (purchase.Reward.TriggeredId != null && purchase.Reward.TriggeredType != null)
            {
                if (purchase.Reward.TriggeredType == "counter")
                {
                    CounterModel? counter = await _context.CounterModel.FindAsync(purchase.Reward.TriggeredId);
                    if (counter != null)
                    {
                        counter.Value += 1;
                    }
                }
                await _context.SaveChangesAsync();
                string? text = purchase.Text;
                if (text != null)
                {
                    bool isMessageText = text.Contains("\nMessage: ");
                    if (isMessageText)
                    {
                        int index = text.IndexOf("\nMessage: ");
                        string newText = text.Substring(index + 10);
                        text = newText;
                        text = HttpUtility.HtmlEncode(text);
                    }
                }
                await _hubService.Trigger(purchase.Reward.TriggeredType, purchase.Reward.TriggeredId, text);
            }
            else
            {
                await _context.SaveChangesAsync();
            }
        }
    }
}
