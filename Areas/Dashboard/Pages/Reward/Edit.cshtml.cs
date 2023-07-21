using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NewStreamSupporter.Data;
using NewStreamSupporter.Helpers;

namespace NewStreamSupporter.Areas.Dashboard.Pages.Rewards
{
    public class EditModel : PageModel
    {
        private readonly NewStreamSupporter.Data.ApplicationContext _context;


        public EditModel(NewStreamSupporter.Data.ApplicationContext context)
        {
            _context = context;
            UserOptions = new List<Tuple<string, string, string>>();
        }

        [BindProperty]
        public RewardModel RewardModel { get; set; } = default!;

        public IList<Tuple<string, string, string>> UserOptions = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null || _context.Rewards == null)
            {
                return NotFound();
            }

            string uid = HttpContext.GetUserId();

            RewardModel? rewardmodel = await _context.Rewards.FirstOrDefaultAsync(m => m.Id == id && m.OwnerId == uid);
            if (rewardmodel == null)
            {
                return NotFound();
            }
            RewardModel = rewardmodel;

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

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string userId = HttpContext.GetUserId();

            if (RewardModel.OwnerId == null || RewardModel.OwnerId != userId)
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
                _context.Attach(RewardModel).State = EntityState.Modified;
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
            else
            {
                _context.Attach(RewardModel).State = EntityState.Modified;
            }



            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RewardModelExists(RewardModel.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool RewardModelExists(string id)
        {
            return (_context.Rewards?.Any(e => e.Id == id && e.OwnerId == HttpContext.GetUserId())).GetValueOrDefault();
        }
    }
}
