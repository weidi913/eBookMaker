using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FYP1.Data;
using FYP1.Models;

namespace FYP1.Pages.BookPages
{
    public class IndexModel : PageModel
    {
        private readonly FYP1.Data.ApplicationDbContext _context;

        public IndexModel(FYP1.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<BookPage> BookPage { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.BookPage != null)
            {
                BookPage = await _context.BookPage
                .Include(b => b.Chapter).ToListAsync();
            }
        }
    }
}
