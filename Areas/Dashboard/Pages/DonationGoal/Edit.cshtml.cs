using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Data;
using NewStreamSupporter.Helpers;
using NewStreamSupporter.Services;

namespace NewStreamSupporter.Areas.Dashboard.Pages.DonationGoal
{
    public class EditModel : PageModel
    {
        private readonly NewStreamSupporter.Data.ApplicationContext _context;
        private readonly DispatcherHubStateService _hubService;

        public EditModel(NewStreamSupporter.Data.ApplicationContext context, DispatcherHubStateService hubService)
        {
            _hubService = hubService;
            _context = context;
        }

        [BindProperty]
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
            DonationGoalModel = donationGoalModel;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (DonationGoalModel.OwnerId == null || DonationGoalModel.OwnerId != HttpContext.GetUserId())
            {
                return Unauthorized();
            }

            DonationGoalModel.TargetAmount = Math.Round(DonationGoalModel.TargetAmount, 2);
            DonationGoalModel.CurrentAmount = Math.Round(DonationGoalModel.CurrentAmount, 2);

            _context.Attach(DonationGoalModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DonationGoalModelExists(DonationGoalModel.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            await _hubService.Reload("donationGoal", DonationGoalModel.Id);

            return RedirectToPage("./Index");
        }

        private bool DonationGoalModelExists(string id)
        {
            return (_context.DonationGoalModel?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
