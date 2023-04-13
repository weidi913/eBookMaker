using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FYP1.Data;
using FYP1.Models;

namespace FYP1.Pages.Elements
{
    public class IndexModel : PageModel
    {
        private readonly FYP1.Data.ApplicationDbContext _context;

        public IndexModel(FYP1.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Element> Element { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Element != null)
            {
                Element = await _context.Element
                .Include(e => e.BookPage).ToListAsync();
            }
        }
    }
}
