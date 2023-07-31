using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewStreamSupporter.Data;
using NewStreamSupporter.Helpers;

namespace NewStreamSupporter.Areas.Dashboard.Pages.Marquee
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
        public MarqueeModel MarqueeModel { get; set; } = default!;


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.Marquees == null || MarqueeModel == null)
            {
                return Page();
            }

            if (MarqueeModel.OwnerId == null)
            {
                MarqueeModel.OwnerId = HttpContext.GetUserId();
            }
            else if (MarqueeModel.OwnerId != HttpContext.GetUserId())
            {
                return Unauthorized();
            }

            MarqueeModel.Delay = Math.Round(MarqueeModel.Delay, 2);
            MarqueeModel.FadeTime = Math.Round(MarqueeModel.FadeTime, 2);
            MarqueeModel.SpeedFactor = Math.Round(MarqueeModel.SpeedFactor, 2);
            MarqueeModel.SpeedFactorPerCharacter = Math.Round(MarqueeModel.SpeedFactorPerCharacter, 2);


            _context.Marquees.Add(MarqueeModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
