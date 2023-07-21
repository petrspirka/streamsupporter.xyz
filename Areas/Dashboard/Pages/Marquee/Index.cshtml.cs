using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Data;
using NewStreamSupporter.Helpers;

namespace NewStreamSupporter.Areas.Dashboard.Pages.Marquee
{
    public class IndexModel : PageModel
    {
        private readonly NewStreamSupporter.Data.ApplicationContext _context;

        public IndexModel(NewStreamSupporter.Data.ApplicationContext context)
        {
            _context = context;
        }

        public IList<MarqueeModel> MarqueeModel { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Marquees != null)
            {
                MarqueeModel = await _context.Marquees.Where(m => m.OwnerId == HttpContext.GetUserId()).ToListAsync();
            }
        }
    }
}
