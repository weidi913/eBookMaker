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

namespace FYP1.Pages.eBooks
{
    public class EditModel : PageModel
    {
        private readonly FYP1.Data.ApplicationDbContext _context;

        public EditModel(FYP1.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public eBook eBook { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.eBook == null)
            {
                return NotFound();
            }

            var ebook =  await _context.eBook.FirstOrDefaultAsync(m => m.bookID == id);
            if (ebook == null)
            {
                return NotFound();
            }
            eBook = ebook;
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

            _context.Attach(eBook).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!eBookExists(eBook.bookID))
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

        private bool eBookExists(int id)
        {
          return (_context.eBook?.Any(e => e.bookID == id)).GetValueOrDefault();
        }
    }
}
