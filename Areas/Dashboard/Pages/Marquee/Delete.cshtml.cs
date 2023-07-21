using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Data;
using NewStreamSupporter.Helpers;
using NewStreamSupporter.Services;

namespace NewStreamSupporter.Areas.Dashboard.Pages.Marquee
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
        public MarqueeModel MarqueeModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null || _context.Marquees == null)
            {
                return NotFound();
            }

            MarqueeModel? marqueemodel = await _context.Marquees.FirstOrDefaultAsync(m => m.Id == id && m.OwnerId == HttpContext.GetUserId());

            if (marqueemodel == null)
            {
                return NotFound();
            }
            else
            {
                MarqueeModel = marqueemodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null || _context.Marquees == null)
            {
                return NotFound();
            }
            MarqueeModel? marqueeModel = await _context.Marquees.Where(m => m.Id == id && m.OwnerId == HttpContext.GetUserId()).FirstOrDefaultAsync();

            if (marqueeModel != null)
            {
                MarqueeModel = marqueeModel;
                _context.Marquees.Remove(MarqueeModel);
                await _context.SaveChangesAsync();
                await _hubService.Reload("marquee", id);
            }

            return RedirectToPage("./Index");
        }
    }
}
