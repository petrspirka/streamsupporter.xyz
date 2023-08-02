using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NewStreamSupporter.Contracts;
using NewStreamSupporter.Data;
using NewStreamSupporter.Services;

namespace NewStreamSupporter.Pages
{
    public class RewardsModel : PageModel
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RewardManagerService _rewardService;
        private readonly ICurrencyService _currencyService;
        private readonly ProfanityFilter.ProfanityFilter _filter;

        public ApplicationUser ShopOwner { get; set; } = default!;
        public ApplicationUser? CurrentUser { get; set; } = default!;
        public ulong? OwnedCurrencyAmount { get; set; } = null;
        public bool? Success { get; set; } = null;
        public Tuple<string, string, bool>? ModalState { get; set; } = null;

        public RewardsModel(ApplicationContext context, UserManager<ApplicationUser> userManager, ICurrencyService currencyService, RewardManagerService rewardService)
        {
            _context = context;
            _userManager = userManager;
            _rewardService = rewardService;
            _currencyService = currencyService;
            _filter = new ProfanityFilter.ProfanityFilter();
        }

        public async Task<IActionResult> OnGetAsync(string uid, bool? success = null)
        {
            ApplicationUser? user = await _context.Users.Where(u => u.Id == uid).Include(u => u.OwnedRewards).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }
            ShopOwner = user;

            Success = success;

            CurrentUser = await _userManager.GetUserAsync(User);
            if (CurrentUser != null)
            {
                OwnedCurrencyAmount = await _currencyService.GetUserCurrencyAmount(CurrentUser.Id, ShopOwner.Id);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id, string uid, string? text = null)
        {
            RewardModel reward = await _context.Rewards.FirstAsync(r => r.Id == id);
            if (reward == null || (!reward.HasTextField && !text.IsNullOrEmpty()))
            {
                return await OnGetAsync(uid, false);
            }
            if (text != null && text.Length > 64)
            {
                return await OnGetAsync(uid, false);
            }
            if (!text.IsNullOrEmpty())
            {
                text = _filter.CensorString(text);
            }
            string? loggedInUser = _userManager.GetUserId(User);
            if (text != null && text.Contains('\n'))
            {
                text = text.Replace("\n", "\\n");
            }
            if (loggedInUser == null || (text != null && text.Length > 64))
            {
                return await OnGetAsync(uid, false);
            }
            try
            {
                return await OnGetAsync(uid, await _rewardService.RedeemReward(loggedInUser, id, text));
            }
            catch (Exception)
            {
                return await OnGetAsync(uid, false);
            }
        }
    }
}
