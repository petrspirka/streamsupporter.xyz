using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Data;
using NewStreamSupporter.Helpers;
using NewStreamSupporter.Services;
using System.Dynamic;

namespace NewStreamSupporter.Areas.Dashboard.Pages.DonationGoal
{
    public class IndexModel : PageModel
    {
        private readonly NewStreamSupporter.Data.ApplicationContext _context;
        private readonly DispatcherHubStateService _hub;

        public IList<DonationGoalModel> DonationGoalModel { get; set; } = default!;

        public IndexModel(NewStreamSupporter.Data.ApplicationContext context, DispatcherHubStateService hub)
        {
            _context = context;
            _hub = hub;
        }

        public async Task<IActionResult> OnPostTestTriggerAsync(string id)
        {
            DonationGoalModel? donationGoalModel = await _context.DonationGoalModel.FirstOrDefaultAsync(a => a.Id == id);
            if (donationGoalModel == null || donationGoalModel.OwnerId != HttpContext.GetUserId())
            {
                return Forbid();
            }
            Random random = new();
            dynamic obj = new ExpandoObject();
            obj.Name = "Test";
            obj.Amount = float.Round(0.5f * (random.NextInt64(10) + 1), 2);
            await _hub.Trigger("donationGoal", donationGoalModel.Id, obj);
            return new EmptyResult();
        }

        private async Task LoadWidgets()
        {
            if (_context.DonationGoalModel != null)
            {
                DonationGoalModel = await _context.DonationGoalModel.Where(t => t.OwnerId == HttpContext.GetUserId()).ToListAsync();
            }
        }

        public async Task OnGetAsync()
        {
            await LoadWidgets();
        }
    }
}
