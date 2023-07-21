using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Data;
using NewStreamSupporter.Helpers;

namespace NewStreamSupporter.Areas.Dashboard.Pages.Rewards
{
    public class DetailsModel : PageModel
    {
        private readonly NewStreamSupporter.Data.ApplicationContext _context;

        public DetailsModel(NewStreamSupporter.Data.ApplicationContext context)
        {
            _context = context;
        }

        public RewardModel RewardModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null || _context.Rewards == null)
            {
                return NotFound();
            }

            RewardModel? rewardmodel = await _context.Rewards.FirstOrDefaultAsync(m => m.Id == id && m.OwnerId == HttpContext.GetUserId());
            if (rewardmodel == null)
            {
                return NotFound();
            }
            else
            {
                RewardModel = rewardmodel;
            }
            return Page();
        }
    }
}
