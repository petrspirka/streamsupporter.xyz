using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Data;
using NewStreamSupporter.Helpers;
using NewStreamSupporter.Services;

namespace NewStreamSupporter.Areas.Dashboard.Pages.Timer
{
    public class EditModel : PageModel
    {
        private readonly NewStreamSupporter.Data.ApplicationContext _context;
        private readonly DispatcherHubStateService _hubService;

        public EditModel(NewStreamSupporter.Data.ApplicationContext context, DispatcherHubStateService hubService)
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
            TimerModel = timermodel;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (HttpContext.GetUserId() == null || TimerModel.OwnerId != HttpContext.GetUserId())
            {
                return Unauthorized();
            }

            _context.Attach(TimerModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TimerModelExists(TimerModel.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            await _hubService.Reload("timer", TimerModel.Id);

            return RedirectToPage("./Index");
        }

        private bool TimerModelExists(string id)
        {
            return (_context.TimerModel?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
