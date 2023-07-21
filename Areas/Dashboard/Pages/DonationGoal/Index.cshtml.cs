using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Data;
using NewStreamSupporter.Helpers;

namespace NewStreamSupporter.Areas.Dashboard.Pages.DonationGoal
{
    public class IndexModel : PageModel
    {
        private readonly NewStreamSupporter.Data.ApplicationContext _context;

        public IndexModel(NewStreamSupporter.Data.ApplicationContext context)
        {
            _context = context;
        }

        public IList<DonationGoalModel> DonationGoalModel { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.DonationGoalModel != null)
            {
                DonationGoalModel = await _context.DonationGoalModel.Where(d => d.OwnerId == HttpContext.GetUserId()).ToListAsync();
            }
        }
    }
}
