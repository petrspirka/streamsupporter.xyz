using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Data;
using NewStreamSupporter.Helpers;
using NewStreamSupporter.Services;

namespace NewStreamSupporter.Areas.Dashboard.Pages.Counter
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
        public CounterModel CounterModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null || _context.CounterModel == null)
            {
                return NotFound();
            }

            CounterModel? countermodel = await _context.CounterModel.FirstOrDefaultAsync(m => m.Id == id && m.OwnerId == HttpContext.GetUserId());

            if (countermodel == null)
            {
                return NotFound();
            }
            else
            {
                CounterModel = countermodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null || _context.CounterModel == null)
            {
                return NotFound();
            }
            CounterModel? countermodel = await _context.CounterModel.Where(c => c.Id == id && c.OwnerId == HttpContext.GetUserId()).FirstOrDefaultAsync();

            if (countermodel != null)
            {
                CounterModel = countermodel;
                _context.CounterModel.Remove(CounterModel);
                await _context.SaveChangesAsync();
            }

            await _hubService.Reload("counter", id);

            return RedirectToPage("./Index");
        }
    }
}
