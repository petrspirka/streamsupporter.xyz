using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Data;
using NewStreamSupporter.Helpers;
using NewStreamSupporter.Services;

namespace NewStreamSupporter.Areas.Dashboard.Pages.DonationGoal
{
    public class DeleteModel : PageModel
    {
        private readonly NewStreamSupporter.Data.ApplicationContext _context;
        private readonly DispatcherHubStateService _hubService;

        public DeleteModel(NewStreamSupporter.Data.ApplicationContext context, DispatcherHubStateService hubService)
        {
            _context = context;
            _hubService = hubService;
        }

        [BindProperty]
        public DonationGoalModel DonationGoalModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null || _context.DonationGoalModel == null)
            {
                return NotFound();
            }

            DonationGoalModel? donationgoalmodel = await _context.DonationGoalModel.FirstOrDefaultAsync(m => m.Id == id && m.OwnerId == HttpContext.GetUserId());

            if (donationgoalmodel == null)
            {
                return NotFound();
            }
            else
            {
                DonationGoalModel = donationgoalmodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null || _context.DonationGoalModel == null)
            {
                return NotFound();
            }
            DonationGoalModel? donationgoalmodel = await _context.DonationGoalModel.FirstOrDefaultAsync(d => d.Id == id && d.OwnerId == HttpContext.GetUserId());

            if (donationgoalmodel != null)
            {
                DonationGoalModel = donationgoalmodel;
                _context.DonationGoalModel.Remove(DonationGoalModel);
                await _context.SaveChangesAsync();
            }

            await _hubService.Reload("donationGoal", id);

            return RedirectToPage("./Index");
        }
    }
}
