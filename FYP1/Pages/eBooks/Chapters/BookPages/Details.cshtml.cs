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
    public class DetailsModel : PageModel
    {
        private readonly FYP1.Data.ApplicationDbContext _context;

        public DetailsModel(FYP1.Data.ApplicationDbContext context)
        {
            _context = context;
        }

      public BookPage BookPage { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Page == null)
            {
                return NotFound();
            }

            var bookpage = await _context.Page.FirstOrDefaultAsync(m => m.bookPageID == id);
            if (bookpage == null)
            {
                return NotFound();
            }
            else 
            {
                BookPage = bookpage;
            }
            return RedirectToPage();
        }
    }
}
