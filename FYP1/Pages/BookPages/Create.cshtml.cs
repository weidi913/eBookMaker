using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FYP1.Data;
using FYP1.Models;

namespace FYP1.Pages.BookPages
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
        ViewData["chapterID"] = new SelectList(_context.Chapter, "chapterID", "chapterName");
            return Page();
        }

        [BindProperty]
        public BookPage BookPage { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.Page == null || BookPage == null)
            {
                return Page();
            }

            _context.Page.Add(BookPage);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
