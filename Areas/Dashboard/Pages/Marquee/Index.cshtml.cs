using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Data;
using NewStreamSupporter.Helpers;
using NewStreamSupporter.Services;

namespace NewStreamSupporter.Areas.Dashboard.Pages.Marquee
{
    public class IndexModel : PageModel
    {
        private readonly NewStreamSupporter.Data.ApplicationContext _context;
        private readonly DispatcherHubStateService _hub;

        public IList<MarqueeModel> MarqueeModel { get; set; } = default!;

        public IndexModel(NewStreamSupporter.Data.ApplicationContext context, DispatcherHubStateService hub)
        {
            _context = context;
            _hub = hub;
        }

        public async Task<IActionResult> OnPostTestTriggerAsync(string id)
        {
            var marquee = await _context.Marquees.FirstOrDefaultAsync(a => a.Id == id);
            if (marquee == null || marquee.OwnerId != HttpContext.GetUserId())
            {
                return Forbid();
            }
            await _hub.Trigger("marquee", marquee.Id, "I am a test trigger");
            return new EmptyResult();
        }

        public async Task OnGetAsync()
        {
            await LoadWidgets();
        }

        private async Task LoadWidgets()
        {
            if (_context.Marquees != null)
            {
                MarqueeModel = await _context.Marquees.Where(m => m.OwnerId == HttpContext.GetUserId()).ToListAsync();
            }
        }
    }
}
