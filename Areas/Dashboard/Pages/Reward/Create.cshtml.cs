using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NewStreamSupporter.Data;
using NewStreamSupporter.Helpers;

namespace NewStreamSupporter.Areas.Dashboard.Pages.Rewards
{
    public class CreateModel : PageModel
    {
        private readonly NewStreamSupporter.Data.ApplicationContext _context;
        public IList<Tuple<string, string, string>> UserOptions { get; set; }

        public CreateModel(NewStreamSupporter.Data.ApplicationContext context)
        {
            _context = context;
            UserOptions = new List<Tuple<string, string, string>>();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            string uid = HttpContext.GetUserId();
            List<MarqueeModel> userMarquees = await _context.Marquees.Where(m => m.OwnerId == uid).ToListAsync();
            AlertModel? userAlert = await _context.Alerts.Where(a => a.OwnerId == uid).FirstOrDefaultAsync();
            List<TimerModel> userTimers = await _context.TimerModel.Where(t => t.OwnerId == uid).ToListAsync();
            List<CounterModel> userCounters = await _context.CounterModel.Where(c => c.OwnerId == uid).ToListAsync();

            foreach (MarqueeModel? marquee in userMarquees)
                UserOptions.Add(new("Marquee", marquee.Id, marquee.Name));

            if (userAlert != null)
                UserOptions.Add(new("Alert", userAlert.Id, userAlert.Name));

            foreach (TimerModel? timer in userTimers)
                UserOptions.Add(new("Timer", timer.Id, timer.Name));

            foreach (CounterModel? counter in userCounters)
            {
                UserOptions.Add(new("Counter", counter.Id, counter.Name));
            }
            return Page();
        }

        [BindProperty]
        public RewardModel RewardModel { get; set; } = default!;


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            string userId = HttpContext.GetUserId();

            if (!ModelState.IsValid || _context.Rewards == null || RewardModel == null)
            {
                return Page();
            }
            else if (RewardModel.OwnerId == null)
            {
                RewardModel.OwnerId = HttpContext.GetUserId();
            }
            else if (RewardModel.OwnerId != HttpContext.GetUserId())
            {
                return Unauthorized();
            }


            string? triggeredElement = HttpContext.Request.Form["chosenElement"].FirstOrDefault();
            if (!triggeredElement.IsNullOrEmpty())
            {
                string[] split = triggeredElement!.Split(':');
                if (split.Length != 2)
                {
                    return Page();
                }
                string type = split[0];
                string id = split[1];
                bool valid = type switch
                {
                    "Marquee" => await _context.Marquees.AnyAsync(m => m.Id == id && m.OwnerId == userId),
                    "Alert" => await _context.Alerts.AnyAsync(a => a.Id == id && a.OwnerId == userId),
                    "Timer" => await _context.TimerModel.AnyAsync(t => t.Id == id && t.OwnerId == userId),
                    "Counter" => await _context.CounterModel.AnyAsync(c => c.Id == id && c.OwnerId == userId),
                    _ => throw new NotImplementedException()
                };

                if (!valid)
                {
                    return Page();
                }

                RewardModel.TriggeredType = type switch
                {
                    "Marquee" => "marquee",
                    "Alert" => "alert",
                    "Timer" => "timer",
                    "Counter" => "counter",
                    _ => throw new NotImplementedException()
                };
                RewardModel.TriggeredId = id;
            }


            _context.Rewards.Add(RewardModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
