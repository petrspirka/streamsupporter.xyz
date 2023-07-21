using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Data;
using NewStreamSupporter.Helpers;
using NewStreamSupporter.Services;

namespace NewStreamSupporter.Areas.Dashboard.Pages.Timer
{
    public class DeleteModel : PageModel
    {
        private readonly NewStreamSupporter.Data.ApplicationContext _context;
        private readonly DispatcherHubStateService _hubService;

        public DeleteModel(NewStreamSupporter.Data.ApplicationContext context, DispatcherHubStateService hubService)
        {
            _context = context;
            _hubService = hubService;
        }

        [BindProperty]
        public TimerModel TimerModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null || _context.TimerModel == null)
            {
                return NotFound();
            }

            TimerModel? timermodel = await _context.TimerModel.FirstOrDefaultAsync(m => m.Id == id && m.OwnerId == HttpContext.GetUserId());

            if (timermodel == null)
            {
                return NotFound();
            }
            else
            {
                TimerModel = timermodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null || _context.TimerModel == null)
            {
                return NotFound();
            }

            TimerModel? timermodel = await _context.TimerModel.FirstOrDefaultAsync(t => t.Id == id && t.OwnerId == HttpContext.GetUserId());

            if (timermodel != null)
            {
                TimerModel = timermodel;
                _context.TimerModel.Remove(TimerModel);
                await _context.SaveChangesAsync();
                await _hubService.Reload("timer", TimerModel.Id);
            }

            return RedirectToPage("./Index");
        }
    }
}
