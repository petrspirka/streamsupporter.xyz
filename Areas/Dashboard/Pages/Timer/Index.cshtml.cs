using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Data;
using NewStreamSupporter.Helpers;
using NewStreamSupporter.Services;

namespace NewStreamSupporter.Areas.Dashboard.Pages.Timer
{
    public class IndexModel : PageModel
    {
        private readonly NewStreamSupporter.Data.ApplicationContext _context;
        private readonly DispatcherHubStateService _hub;

        public IndexModel(NewStreamSupporter.Data.ApplicationContext context, DispatcherHubStateService hub)
        {
            _context = context;
            _hub = hub;
        }

        public async Task<IActionResult> OnPostTestTriggerAsync(string id)
        {
            var timer = await _context.TimerModel.FirstOrDefaultAsync(a => a.Id == id);
            if (timer == null || timer.OwnerId != HttpContext.GetUserId())
            {
                return Forbid();
            }
            await _hub.Trigger("timer", timer.Id, null);
            return new EmptyResult();
        }

        private async Task LoadWidgets()
        {
            if (_context.TimerModel != null)
            {
                TimerModel = await _context.TimerModel.Where(t => t.OwnerId == HttpContext.GetUserId()).ToListAsync();
            }
        }

        public IList<TimerModel> TimerModel { get; set; } = default!;

        public async Task OnGetAsync()
        {
            await LoadWidgets();
        }
    }
}
