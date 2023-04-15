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

namespace FYP1.Pages.Chapters
{
    public class EditModel : PageModel
    {
        private readonly FYP1.Data.ApplicationDbContext _context;

        public EditModel(FYP1.Data.ApplicationDbContext context)
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

            var chapter =  await _context.Chapter.FirstOrDefaultAsync(m => m.chapterID == id);
            if (chapter == null)
            {
                return NotFound();
            }
            Chapter = chapter;
           ViewData["bookID"] = new SelectList(_context.Set<eBook>(), "bookID", "background");
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

            _context.Attach(Chapter).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChapterExists(Chapter.chapterID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToPage("../Details", new { id = Chapter.bookID });
            return RedirectToAction("OnGetAsync", new { id = Chapter.bookID }); ;
        }

        private bool ChapterExists(int id)
        {
          return (_context.Chapter?.Any(e => e.chapterID == id)).GetValueOrDefault();
        }
    }
}
