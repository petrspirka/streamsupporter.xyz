using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Contracts;
using NewStreamSupporter.Data;
using NewStreamSupporter.Helpers;
using NewStreamSupporter.Services;

namespace NewStreamSupporter.Areas.Dashboard.Pages.Alert
{
    public class AlertPageModel : PageModel
    {
        private readonly DispatcherHubStateService _hubService;
        private readonly NewStreamSupporter.Data.ApplicationContext _context;
        private readonly IFileStore _store;

        public AlertPageModel(NewStreamSupporter.Data.ApplicationContext context, DispatcherHubStateService hubService, IFileStore store)
        {
            _hubService = hubService;
            _context = context;
            _store = store;
        }

        [BindProperty]
        public AlertModel AlertModel { get; set; } = default!;
        public string FileStatusMessage { get; set; } = "";

        public async Task<IActionResult> OnPostClearFile()
        {
            await _store.Delete(HttpContext.GetUserId());
            return Page();
        }

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

            if(HttpContext.Request.Form.Files.Count == 1)
            {
                var file = HttpContext.Request.Form.Files[0];
                if (!file.ContentType.Contains("audio"))
                {
                    FileStatusMessage = "Invalid file specified";
                    return Page();
                }
                await _store.Store(AlertModel.OwnerId, file.OpenReadStream());
                AlertModel.AudioType = file.ContentType;
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
