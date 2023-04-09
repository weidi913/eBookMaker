using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FYP1.Data;
using FYP1.Models;

namespace FYP1.Pages.BookPages
{
    public class EditModel : PageModel
    {
        private readonly FYP1.Data.ApplicationDbContext _context;

        public EditModel(FYP1.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public BookPage BookPage { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Page == null)
            {
                return NotFound();
            }

            var bookpage =  await _context.Page.FirstOrDefaultAsync(m => m.bookPageID == id);
            if (bookpage == null)
            {
                return NotFound();
            }
            BookPage = bookpage;
           ViewData["chapterID"] = new SelectList(_context.Chapter, "chapterID", "chapterName");
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

            _context.Attach(BookPage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookPageExists(BookPage.bookPageID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool BookPageExists(int id)
        {
          return (_context.Page?.Any(e => e.bookPageID == id)).GetValueOrDefault();
        }
    }
}
