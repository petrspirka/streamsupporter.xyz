using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewStreamSupporter.Data;
using NewStreamSupporter.Helpers;

namespace NewStreamSupporter.Areas.Dashboard.Pages.Counter
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
        public CounterModel CounterModel { get; set; } = default!;


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.CounterModel == null || CounterModel == null)
            {
                return Page();
            }

            if (CounterModel.OwnerId == null)
            {
                CounterModel.OwnerId = HttpContext.GetUserId();
            }
            else if (CounterModel.OwnerId != HttpContext.GetUserId() || CounterModel.Id != null)
            {
                return Unauthorized();
            }

            _context.CounterModel.Add(CounterModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
