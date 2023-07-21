using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Data;
using NewStreamSupporter.Helpers;

namespace NewStreamSupporter.Areas.Dashboard.Pages.Timer
{
    public class DetailsModel : PageModel
    {
        private readonly NewStreamSupporter.Data.ApplicationContext _context;

        public DetailsModel(NewStreamSupporter.Data.ApplicationContext context)
        {
            _context = context;
        }

        public TimerModel TimerModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null || _context.TimerModel == null)
            {
                return NotFound();
            }

            TimerModel? timermodel = await _context.TimerModel.FirstOrDefaultAsync(m => m.Id == id && m.OwnerId == HttpContext.GetUserId());
            if (timermodel == null)
            {
                return NotFound();
            }
            else
            {
                TimerModel = timermodel;
            }
            return Page();
        }
    }
}
