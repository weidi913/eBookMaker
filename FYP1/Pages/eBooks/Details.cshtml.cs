using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FYP1.Data;
using FYP1.Models;

namespace FYP1.Pages.eBooks
{
    public class DetailsModel : PageModel
    {
        private readonly FYP1.Data.ApplicationDbContext _context;

        public DetailsModel(FYP1.Data.ApplicationDbContext context)
        {
            _context = context;
        }

      public eBook eBook { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.eBook == null)
            {
                return NotFound();
            }

            var ebook = await _context.eBook.FirstOrDefaultAsync(m => m.bookID == id);
            if (ebook == null)
            {
                return NotFound();
            }
            else 
            {
                eBook = ebook;
            }
            return Page();
        }
    }
}
