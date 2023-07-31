using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Contracts;
using NewStreamSupporter.Data;
using NewStreamSupporter.Helpers;
using NewStreamSupporter.Services;
using TwitchLib.Api.Core.Exceptions;

namespace NewStreamSupporter.Areas.Dashboard.Pages.Alert
{
    public class AlertPageModel : PageModel
    {
        private readonly DispatcherHubStateService _hubService;
        private readonly NewStreamSupporter.Data.ApplicationContext _context;
        private readonly IFileStore _store;
        private readonly DispatcherHubStateService _hub;

        public AlertPageModel(NewStreamSupporter.Data.ApplicationContext context, DispatcherHubStateService hubService, IFileStore store, DispatcherHubStateService hub)
        {
            _hubService = hubService;
            _context = context;
            _store = store;
            _hub = hub;
        }

        [BindProperty]
        public AlertModel AlertModel { get; set; } = default!;
        public string FileStatusMessage { get; set; } = "";

        public async Task<IActionResult> OnPostClearFileAsync()
        {
            await _store.Delete(HttpContext.GetUserId());
            await _hubService.Reload("alert", AlertModel.Id);
            return Page();
        }

        public async Task<IActionResult> OnPostTestTriggerAsync()
        {
            var form = HttpContext.Request.Form;
            var alert = await _context.Alerts.FirstOrDefaultAsync(a => a.Id == form["AlertModel.Id"].ToString());
            if (alert == null || alert.OwnerId != HttpContext.GetUserId())
            {
                return Forbid();
            }
            await _hub.Trigger("alert", alert.Id, "I am a test trigger");
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

            if (HttpContext.Request.Form.Files.Count == 1)
            {
                var file = HttpContext.Request.Form.Files[0];
                if (file.Length > 2000000)
                {
                    FileStatusMessage = "Maximum file size exceeded.";
                    return Page();
                }
                if (!file.ContentType.Contains("audio"))
                {
                    FileStatusMessage = "Specified file is not a valid audio file.";
                    return Page();
                }
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                var array = stream.ToArray();
                var valid = VerifySignature(file, array);
                if (!valid)
                {
                    FileStatusMessage = "Invalid file signature detected. Audio files must be of mp3 or wav format.";
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

        private bool VerifySignature(IFormFile file, byte[] array)
        {
            if (array.Length < 1)
            {
                return false;
            }
            switch (Path.GetExtension(file.FileName))
            {
                case ".mp3":
                    return array.Length >= 3 && (
                        (array[0] == 'I' &&
                            array[1] == 'D' &&
                            array[2] == '3') 
                            || 
                            (array[0] == 255 &&
                            (array[1] & 0xE0) == 224 &&
                            (array[2] & 0xF0) == 240)
                        );
                case ".wav":
                    return array.Length >= 4 &&
                        array[0] == 'R' &&
                        array[1] == 'I' &&
                        array[2] == 'F' &&
                        array[3] == 'F';
                default:
                    return false;
            }
        }
    }
}
