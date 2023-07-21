using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Data;
using NewStreamSupporter.Helpers;

namespace NewStreamSupporter.Areas.Dashboard.Pages.DonationGoal
{
    public class DetailsModel : PageModel
    {
        private readonly NewStreamSupporter.Data.ApplicationContext _context;

        public DetailsModel(NewStreamSupporter.Data.ApplicationContext context)
        {
            _context = context;
        }

        public DonationGoalModel DonationGoalModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null || _context.DonationGoalModel == null)
            {
                return NotFound();
            }

            DonationGoalModel? donationGoalModel = await _context.DonationGoalModel.FirstOrDefaultAsync(m => m.Id == id && m.OwnerId == HttpContext.GetUserId());
            if (donationGoalModel == null)
            {
                return NotFound();
            }
            else
            {
                DonationGoalModel = donationGoalModel;
            }
            return Page();
        }
    }
}
