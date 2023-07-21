using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Data;
using NewStreamSupporter.Helpers;

namespace NewStreamSupporter.Areas.Dashboard.Pages.Marquee
{
    public class DetailsModel : PageModel
    {
        private readonly NewStreamSupporter.Data.ApplicationContext _context;

        public DetailsModel(NewStreamSupporter.Data.ApplicationContext context)
        {
            _context = context;
        }

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
    }
}
