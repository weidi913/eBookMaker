using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FYP1.Data;
using FYP1.Models;

namespace FYP1.Pages.Elements
{
    public class CreateModel : PageModel
    {
        private readonly FYP1.Data.ApplicationDbContext _context;

        public CreateModel(FYP1.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["bookPageID"] = new SelectList(_context.Page, "bookPageID", "bookPageID");
            return Page();
        }

        [BindProperty]
        public Element Element { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.Element == null || Element == null)
            {
                return Page();
            }

            _context.Element.Add(Element);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
