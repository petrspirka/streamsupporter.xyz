using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Data;
using NewStreamSupporter.Helpers;
using NewStreamSupporter.Services;
using System.Dynamic;

namespace NewStreamSupporter.Areas.Dashboard.Pages.Counter
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

        public IList<CounterModel> CounterModel { get; set; } = default!;

        public async Task<IActionResult> OnPostTestTriggerAsync(string id)
        {
            var counter = await _context.CounterModel.FirstOrDefaultAsync(a => a.Id == id);
            if (counter == null || counter.OwnerId != HttpContext.GetUserId())
            {
                return Forbid();
            }
            await _hub.Trigger("counter", counter.Id, null);
            return new EmptyResult();
        }

        private async Task LoadWidgets()
        {
            if (_context.CounterModel != null)
            {
                CounterModel = await _context.CounterModel.Where(c => c.OwnerId == HttpContext.GetUserId()).ToListAsync();
            }
        }

        public async Task OnGetAsync()
        {
            await LoadWidgets();
        }
    }
}
