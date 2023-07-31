using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewStreamSupporter.Data;
using NewStreamSupporter.Helpers;

namespace NewStreamSupporter.Areas.Dashboard.Pages.Timer
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
        public TimerModel TimerModel { get; set; } = default!;


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.TimerModel == null || TimerModel == null)
            {
                return Page();
            }

            if (TimerModel.OwnerId == null)
            {
                TimerModel.OwnerId = HttpContext.GetUserId();
            }
            else if (TimerModel.OwnerId != HttpContext.GetUserId())
            {
                return Unauthorized();
            }

            _context.TimerModel.Add(TimerModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
