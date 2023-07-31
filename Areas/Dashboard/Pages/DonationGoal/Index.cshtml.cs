using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Data;
using NewStreamSupporter.Helpers;
using NewStreamSupporter.Services;
using System.Dynamic;
using TwitchLib.Api.Helix;

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

        public async Task<IActionResult> OnPostTestTriggerAsync()
        {
            var form = HttpContext.Request.Form;
            var donationGoalModel = await _context.DonationGoalModel.FirstOrDefaultAsync(a => a.Id == form["DonationGoalModel.Id"].ToString());
            if (donationGoalModel == null || donationGoalModel.OwnerId != HttpContext.GetUserId())
            {
                return Forbid();
            }
            var random = new Random();
            dynamic obj = new ExpandoObject();
            obj.Name = "Test";
            obj.Amount = float.Round(0.5f * (random.NextInt64(10)+1), 2);
            await _hub.Trigger("donationGoal", donationGoalModel.Id, obj);
            await LoadWidgets();
            return Page();
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
