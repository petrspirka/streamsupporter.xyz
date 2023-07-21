using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewStreamSupporter.Data;
using NewStreamSupporter.Helpers;
using NewStreamSupporter.Services;
using System.ComponentModel.DataAnnotations;

namespace NewStreamSupporter.Areas.Dashboard.Pages
{
    public class OptionsModel : PageModel
    {
        private readonly ApplicationContext _context;
        private readonly TwitchListenerService _twitchListenerService;
        private readonly YouTubeListenerService _youTubeListenerService;


        [BindProperty]
        public InputModel Model { get; set; } = default!;


        public OptionsModel(ApplicationContext context, TwitchListenerService twitchListenerService, YouTubeListenerService youTubeListenerService)
        {
            _context = context;
            _twitchListenerService = twitchListenerService;
            _youTubeListenerService = youTubeListenerService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            ApplicationUser applicationUser = await HttpContext.GetUser(_context);
            if (applicationUser == null)
            {
                return Unauthorized();
            }

            Model = new()
            {
                IsGoogleActive = applicationUser.IsGoogleActive,
                IsTwitchActive = applicationUser.IsTwitchActive,
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ApplicationUser applicationUser = await HttpContext.GetUser(_context);
            if (!ModelState.IsValid || applicationUser == null)
            {
                return Page();
            }

            applicationUser.IsTwitchActive = Model.IsTwitchActive;
            applicationUser.IsGoogleActive = Model.IsGoogleActive;

            if (applicationUser.IsGoogleActive && applicationUser.GoogleBrandId != null)
            {
                await _youTubeListenerService.AddAllUserListeners(applicationUser.GoogleBrandId);
            }
            else if (applicationUser.GoogleBrandId != null)
            {
                await _youTubeListenerService.RemoveAllUserListeners(applicationUser.GoogleBrandId);
            }

            if (applicationUser.IsTwitchActive && applicationUser.TwitchId != null)
            {
                await _twitchListenerService.AddAllUserListeners(applicationUser.TwitchId);
            }
            else if (applicationUser.TwitchId != null)
            {
                await _twitchListenerService.RemoveAllUserListeners(applicationUser.TwitchId);
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }

        public class InputModel
        {
            [Display(Name = "Is Twitch enabled")]
            public bool IsTwitchActive { get; set; } = default!;

            [Display(Name = "Is Google enabled")]
            public bool IsGoogleActive { get; set; } = default!;
        }
    }
}
