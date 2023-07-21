using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Data;
using NewStreamSupporter.Helpers;
using NewStreamSupporter.Services;

namespace NewStreamSupporter.Areas.Dashboard.Pages.Alert
{
    public class AlertPageModel : PageModel
    {
        private readonly DispatcherHubStateService _hubService;
        private readonly NewStreamSupporter.Data.ApplicationContext _context;

        public AlertPageModel(NewStreamSupporter.Data.ApplicationContext context, DispatcherHubStateService hubService)
        {
            _hubService = hubService;
            _context = context;
        }

        [BindProperty]
        public AlertModel AlertModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            ApplicationUser user = await HttpContext.GetUser(_context);
            AlertModel? alertModel = await _context.Alerts.FirstOrDefaultAsync(m => m.OwnerId == user.Id);
            if (alertModel == null)
            {
                alertModel = new AlertModel
                {
                    Owner = user
                };
                _context.Alerts.Add(alertModel);
                _context.SaveChanges();
                alertModel = await _context.Alerts.FirstOrDefaultAsync(m => m.OwnerId == user.Id);
            }
            AlertModel = alertModel!;
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

            _context.Attach(AlertModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlertModelExists(AlertModel.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            await _hubService.Reload("alert", AlertModel.Id);

            return RedirectToPage("./Alert");
        }

        private bool AlertModelExists(string id)
        {
            return (_context.Alerts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
