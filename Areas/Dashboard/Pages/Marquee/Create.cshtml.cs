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
            if (MarqueeModel.Owner == null)
            {
                MarqueeModel.Owner = await HttpContext.GetUser(_context);
            }
            else if (MarqueeModel.Owner != await HttpContext.GetUser(_context))
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid || _context.Marquees == null || MarqueeModel == null)
            {
                return Page();
            }

            _context.Marquees.Add(MarqueeModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
