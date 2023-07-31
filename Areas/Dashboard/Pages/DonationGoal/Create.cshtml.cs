using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewStreamSupporter.Data;
using NewStreamSupporter.Helpers;

namespace NewStreamSupporter.Areas.Dashboard.Pages.DonationGoal
{
    public class CreateModel : PageModel
    {
        private readonly NewStreamSupporter.Data.ApplicationContext _context;

        public CreateModel(NewStreamSupporter.Data.ApplicationContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public DonationGoalModel DonationGoalModel { get; set; } = default!;


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.DonationGoalModel == null || DonationGoalModel == null)
            {
                return Page();
            }

            if(DonationGoalModel.OwnerId == null)
            {
                DonationGoalModel.OwnerId = HttpContext.GetUserId();
            }
            else if (DonationGoalModel.OwnerId != HttpContext.GetUserId())
            {
                return Unauthorized();
            }

            DonationGoalModel.TargetAmount = Math.Round(DonationGoalModel.TargetAmount, 2);
            DonationGoalModel.CurrentAmount = Math.Round(DonationGoalModel.CurrentAmount, 2);

            _context.DonationGoalModel.Add(DonationGoalModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
