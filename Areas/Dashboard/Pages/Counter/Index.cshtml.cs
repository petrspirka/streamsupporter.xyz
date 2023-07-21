using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Data;
using NewStreamSupporter.Helpers;

namespace NewStreamSupporter.Areas.Dashboard.Pages.Counter
{
    public class IndexModel : PageModel
    {
        private readonly NewStreamSupporter.Data.ApplicationContext _context;

        public IndexModel(NewStreamSupporter.Data.ApplicationContext context)
        {
            _context = context;
        }

        public IList<CounterModel> CounterModel { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.CounterModel != null)
            {
                CounterModel = await _context.CounterModel.Where(c => c.OwnerId == HttpContext.GetUserId()).ToListAsync();
            }
        }
    }
}
