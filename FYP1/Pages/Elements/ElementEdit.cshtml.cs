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

namespace FYP1.Pages.Elements
{
    public class EditModel : PageModel
    {
        private readonly FYP1.Data.ApplicationDbContext _context;

        public EditModel(FYP1.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Element Element { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Element == null)
            {
                return NotFound();
            }

            var element =  await _context.Element.FirstOrDefaultAsync(m => m.elementID == id);
            if (element == null)
            {
                return NotFound();
            }
            Element = element;
           ViewData["bookPageID"] = new SelectList(_context.BookPage, "bookPageID", "bookPageID");
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

            _context.Attach(Element).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ElementExists(Element.elementID))
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

        private bool ElementExists(int id)
        {
          return (_context.Element?.Any(e => e.elementID == id)).GetValueOrDefault();
        }
    }
}
