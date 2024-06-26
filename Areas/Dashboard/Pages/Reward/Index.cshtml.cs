﻿using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NewStreamSupporter.Data;
using NewStreamSupporter.Helpers;

namespace NewStreamSupporter.Areas.Dashboard.Pages.Rewards
{
    public class IndexModel : PageModel
    {
        private readonly NewStreamSupporter.Data.ApplicationContext _context;

        public IndexModel(NewStreamSupporter.Data.ApplicationContext context)
        {
            _context = context;
        }

        public IList<RewardModel> RewardModel { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Rewards != null)
            {
                RewardModel = await _context.Rewards.Where(m => m.OwnerId == HttpContext.GetUserId()).ToListAsync();
            }
        }
    }
}
