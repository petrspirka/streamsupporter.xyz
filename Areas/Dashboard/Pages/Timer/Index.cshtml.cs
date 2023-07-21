using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Data;
using NewStreamSupporter.Helpers;

namespace NewStreamSupporter.Areas.Dashboard.Pages.Timer
{
    public class IndexModel : PageModel
    {
        private readonly NewStreamSupporter.Data.ApplicationContext _context;

        public IndexModel(NewStreamSupporter.Data.ApplicationContext context)
        {
            _context = context;
        }

        public IList<TimerModel> TimerModel { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.TimerModel != null)
            {
                TimerModel = await _context.TimerModel.Where(t => t.OwnerId == HttpContext.GetUserId()).ToListAsync();
            }
        }
    }
}
