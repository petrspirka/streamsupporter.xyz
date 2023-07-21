using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Data;
using NewStreamSupporter.Helpers;

namespace NewStreamSupporter.Areas.Dashboard.Pages.Counter
{
    public class DetailsModel : PageModel
    {
        private readonly NewStreamSupporter.Data.ApplicationContext _context;

        public DetailsModel(NewStreamSupporter.Data.ApplicationContext context)
        {
            _context = context;
        }

        public CounterModel CounterModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null || _context.CounterModel == null)
            {
                return NotFound();
            }

            CounterModel? counterModel = await _context.CounterModel.FirstOrDefaultAsync(m => m.Id == id && m.OwnerId == HttpContext.GetUserId());
            if (counterModel == null)
            {
                return NotFound();
            }
            else
            {
                CounterModel = counterModel;
            }
            return Page();
        }
    }
}
