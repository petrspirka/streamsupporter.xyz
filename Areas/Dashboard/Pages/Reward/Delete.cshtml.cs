using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Data;
using NewStreamSupporter.Helpers;

namespace NewStreamSupporter.Areas.Dashboard.Pages.Rewards
{
    public class DeleteModel : PageModel
    {
        private readonly NewStreamSupporter.Data.ApplicationContext _context;

        public DeleteModel(NewStreamSupporter.Data.ApplicationContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null || _context.Rewards == null)
            {
                return NotFound();
            }
            RewardModel? rewardmodel = await _context.Rewards.Where(r => r.Id == id && r.OwnerId == HttpContext.GetUserId()).FirstOrDefaultAsync();

            if (rewardmodel != null)
            {
                RewardModel = rewardmodel;
                _context.Rewards.Remove(RewardModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
