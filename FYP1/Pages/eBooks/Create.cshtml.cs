using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FYP1.Data;
using FYP1.Models;

namespace FYP1.Pages.eBooks
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
            return Page();
        }

        [BindProperty]
        public eBook eBook { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.eBook == null || eBook == null)
            {
                return Page();
            }

            
            _context.eBook.Add(eBook);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
