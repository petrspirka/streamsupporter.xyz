using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Data;

namespace NewStreamSupporter.Areas.Api.Controllers
{
    [Area("Api")]
    public class ComponentController : Controller
    {
        private readonly ApplicationContext _context;

        public ComponentController(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Alert(string uid, string id)
        {
            AlertModel? alert = await _context.Alerts.Where(a => a.Id == id && a.Owner.Id == uid).FirstOrDefaultAsync();
            if (alert == null)
            {
                return NotFound();
            }
            return View("AlertView", alert);
        }

        public async Task<IActionResult> Marquee(string uid, string id)
        {
            ApplicationUser? user = await _context.Users.Where(user => user.Id == uid).Include(user => user.OwnedMarquees).FirstOrDefaultAsync();
            MarqueeModel? marquee = user?.OwnedMarquees.Where(marquee => marquee.Id == id).FirstOrDefault();
            if (marquee == null)
            {
                return NotFound();
            }
            return View("MarqueeView", marquee);
        }

        public async Task<IActionResult> DonationGoal(string uid, string id)
        {
            ApplicationUser? user = await _context.Users.Where(user => user.Id == uid).Include(user => user.OwnedDonationGoals).FirstOrDefaultAsync();
            DonationGoalModel? donationGoal = user?.OwnedDonationGoals.Where(donationGoal => donationGoal.Id == id).FirstOrDefault();
            if (donationGoal == null)
            {
                return NotFound();
            }
            return View("DonationGoalView", donationGoal);
        }

        public async Task<IActionResult> Timer(string uid, string id)
        {
            ApplicationUser? user = await _context.Users.Where(user => user.Id == uid).Include(user => user.OwnedTimers).FirstOrDefaultAsync();
            TimerModel? timer = user?.OwnedTimers.Where(timer => timer.Id == id).FirstOrDefault();
            if (timer == null)
            {
                return NotFound();
            }
            return View("TimerView", timer);
        }

        public async Task<IActionResult> Counter(string uid, string id)
        {
            ApplicationUser? user = await _context.Users.Where(user => user.Id == uid).Include(user => user.OwnedCounters).FirstOrDefaultAsync();
            CounterModel? counter = user?.OwnedCounters.Where(counter => counter.Id == id).FirstOrDefault();
            if (counter == null)
            {
                return NotFound();
            }
            return View("CounterView", counter);
        }

        public ActionResult Index()
        {
            return NotFound();
        }
    }
}
