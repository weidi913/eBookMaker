using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FYP1.Data;
using FYP1.Models;

namespace FYP1.Pages.Chapters
{
    public class DeleteModel : PageModel
    {
        private readonly FYP1.Data.ApplicationDbContext _context;

        public DeleteModel(FYP1.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Chapter Chapter { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Chapter == null)
            {
                return NotFound();
            }

            var chapter = await _context.Chapter.FirstOrDefaultAsync(m => m.chapterID == id);

            if (chapter == null)
            {
                return NotFound();
            }
            else 
            {
                Chapter = chapter;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Chapter == null)
            {
                return NotFound();
            }
            var chapter = await _context.Chapter.FindAsync(id);

            if (chapter != null)
            {
                Chapter = chapter;
                _context.Chapter.Remove(Chapter);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
