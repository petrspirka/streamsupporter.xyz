using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Data;
using NewStreamSupporter.Data.Abstractions;
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
        public string SelectedWidget { get; set; } = default!;

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

            SelectedWidget = "None";

            if (RewardModel.TriggeredId != null && RewardModel.TriggeredType != null)
            {
                var type = char.ToUpper(RewardModel.TriggeredType[0]) + RewardModel.TriggeredType[1..];
                var triggeredWidget = await GetWidget(type, RewardModel.TriggeredId);
                if(triggeredWidget != null)
                {
                    SelectedWidget = $"{type} - {triggeredWidget.Name}";
                }
            }

            return Page();
        }

        private async Task<BaseComponentModel?> GetWidget(string type, string id)
        {
            switch (type)
            {
                case "Marquee":
                    return await _context.Marquees.Where(m => m.Id == id && m.OwnerId == HttpContext.GetUserId()).FirstOrDefaultAsync();
                case "Counter":
                    return await _context.CounterModel.Where(c => c.Id == id && c.OwnerId == HttpContext.GetUserId()).FirstOrDefaultAsync();
                case "Alert":
                    return await _context.Alerts.Where(a => a.Id == id && a.OwnerId == HttpContext.GetUserId()).FirstOrDefaultAsync();
                case "Timer":
                    return await _context.TimerModel.Where(t => t.Id == id && t.OwnerId == HttpContext.GetUserId()).FirstOrDefaultAsync();
                default:
                    return null;
            }
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
