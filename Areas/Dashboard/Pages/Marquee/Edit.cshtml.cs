﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Data;
using NewStreamSupporter.Helpers;
using NewStreamSupporter.Services;

namespace NewStreamSupporter.Areas.Dashboard.Pages.Marquee
{
    public class EditModel : PageModel
    {
        private readonly NewStreamSupporter.Data.ApplicationContext _context;
        private readonly DispatcherHubStateService _hubService;

        public EditModel(NewStreamSupporter.Data.ApplicationContext context, DispatcherHubStateService hubService)
        {
            _context = context;
            _hubService = hubService;
        }

        [BindProperty]
        public MarqueeModel MarqueeModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null || _context.Marquees == null)
            {
                return NotFound();
            }

            MarqueeModel? marqueeModel = await _context.Marquees.FirstOrDefaultAsync(m => m.Id == id && m.OwnerId == HttpContext.GetUserId());
            if (marqueeModel == null)
            {
                return NotFound();
            }
            MarqueeModel = marqueeModel;
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

            if (MarqueeModel.OwnerId == null || MarqueeModel.OwnerId != HttpContext.GetUserId())
            {
                return Unauthorized();
            }

            MarqueeModel.Delay = Math.Round(MarqueeModel.Delay * 20) / 20;
            MarqueeModel.FadeTime = Math.Round(MarqueeModel.FadeTime * 20) / 20;
            MarqueeModel.SpeedFactor = Math.Round(MarqueeModel.SpeedFactor * 20) / 20;
            MarqueeModel.SpeedFactorPerCharacter = Math.Round(MarqueeModel.SpeedFactorPerCharacter * 20) / 20;

            _context.Attach(MarqueeModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MarqueeModelExists(MarqueeModel.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            await _hubService.Reload("marquee", MarqueeModel.Id);
            return RedirectToPage("./Index");
        }

        private bool MarqueeModelExists(string id)
        {
            return (_context.Marquees?.Any(e => e.Id == id && e.OwnerId == HttpContext.GetUserId())).GetValueOrDefault();
        }
    }
}
