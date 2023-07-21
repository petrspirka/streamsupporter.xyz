using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Data;
using NewStreamSupporter.Helpers;
using NewStreamSupporter.Services;

namespace NewStreamSupporter.Areas.Dashboard.Pages.Marquee
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
        public MarqueeModel MarqueeModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null || _context.Marquees == null)
            {
                return NotFound();
            }

            MarqueeModel? marqueeModel = await _context.Marquees.FirstOrDefaultAsync(m => m.Id == id && m.OwnerId == HttpContext.GetUserId());
            if (marqueeModel == null)
            {
                return NotFound();
            }
            MarqueeModel = marqueeModel;
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

            if (MarqueeModel.Owner != null && MarqueeModel.OwnerId != HttpContext.GetUserId())
            {
                return Unauthorized();
            }

            _context.Attach(MarqueeModel).State = EntityState.Modified;

            if (MarqueeModel.Owner == null)
            {
                MarqueeModel.Owner = await HttpContext.GetUser(_context);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MarqueeModelExists(MarqueeModel.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            await _hubService.Reload("marquee", MarqueeModel.Id);
            return RedirectToPage("./Index");
        }

        private bool MarqueeModelExists(string id)
        {
            return (_context.Marquees?.Any(e => e.Id == id && e.OwnerId == HttpContext.GetUserId())).GetValueOrDefault();
        }
    }
}
