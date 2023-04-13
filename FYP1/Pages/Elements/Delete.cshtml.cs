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
    
    public class DeleteModel : PageModel
    {
        private readonly FYP1.Data.ApplicationDbContext _context;

        public DeleteModel(FYP1.Data.ApplicationDbContext context)
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

            var element = await _context.Element.FirstOrDefaultAsync(m => m.elementID == id);

            if (element == null)
            {
                return NotFound();
            }
            else 
            {
                Element = element;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Element == null)
            {
                return NotFound();
            }
            var element = await _context.Element.FindAsync(id);

            if (element != null)
            {
                Element = element;
                _context.Element.Remove(Element);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
