using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FYP1.Data;
using FYP1.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FYP1.Pages.eBooks
{
    public class DetailsModel : PageModel
    {
        private readonly FYP1.Data.ApplicationDbContext _context;

        public DetailsModel(FYP1.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty] //really need it?
        public eBook eBook { get; set; } = default!;
        public IList<Chapter> Chapter { get; set; } = default!;
        public IList<BookPage> BookPage { get; set; } = default!;
        public IList<Element> Element { get; set; } = default!;

        [BindProperty]
        public Chapter ChapterAdd { get; set; } = default!;

        [BindProperty]
        public BookPage BookPageAdd { get; set; } = default!;
        [BindProperty]
        public Element ElementAdd { get; set; } = default!;


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.eBook == null)
            {
                return NotFound();
            }

            var ebook = await _context.eBook.FirstOrDefaultAsync(m => m.bookID == id);
            if (ebook == null)
            {
                return NotFound();
            }
            else
            {
                eBook = ebook;
                ChapterAdd = new Chapter();
                ChapterAdd.bookID = eBook.bookID;
            }
            //return Page();

            if (_context.Chapter != null)
            {
                Chapter = await _context.Chapter
                .Include(c => c.book).ToListAsync();
            }

            if (_context.Page != null)
            {
                BookPage = await _context.Page
                .Include(b => b.Chapter).ToListAsync();
            }

            if (_context.Element != null)
            {
                Element = await _context.Element
                .Include(e => e.BookPage).ToListAsync();
            }


            //no need to filter???? i think need to filter to avoid excessive loadinf time
            ViewData["bookID"] = new SelectList(_context.Set<eBook>(), "bookID", "background");
            ViewData["chapterID"] = new SelectList(_context.Chapter, "chapterID", "chapterName");
            return Page();

        }
/*        public async Task OnGetAsync()
        {
            if (_context.Element != null)
            {
                Element = await _context.Element
                .Include(e => e.BookPage).ToListAsync();
            }
        }*/

        //public IActionResult OnGet()
        //{
        //    ViewData["bookID"] = new SelectList(_context.Set<eBook>(), "bookID", "background");
        //    return Page();
        //}


        //public IList<Chapter> Chapter { get; set; } = default!;

        //public async Task OnGetAsync()
        //{
        //    if (_context.Chapter != null)
        //    {
        //        Chapter = await _context.Chapter
        //        .Include(c => c.book).ToListAsync();
        //    }
        //}


        /*
         * 
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
         */

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostChapterAsync()
        {
            if (!ModelState.IsValid || _context.Chapter == null || ChapterAdd == null)
            {
            //    return Page();
            }

            _context.Chapter.Add(ChapterAdd);
            await _context.SaveChangesAsync();
            return RedirectToAction("OnGetAsync", new { id = ChapterAdd.bookID }); ;
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostBookPageAsync()
        {
            //if (!ModelState.IsValid || _context.Page == null || BookPage == null)
            //{
            //    return Page();
            //}
            _context.Page.Add(BookPageAdd);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }




        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostElementAsync()
        {
/*            if (!ModelState.IsValid || _context.Element == null || Element == null)
            {
                return Page();
            }*/

            _context.Element.Add(ElementAdd);
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true });
/*            return RedirectToPage("./Index");*//**/
        }

        //public async Task<IActionResult> OnPostAsync()
        //{
        //    if (!ModelState.IsValid || _context.Chapter == null || ChapterAdd == null)
        //    {
        //        return Page();
        //    }

        //    _context.Chapter.Add(ChapterAdd);
        //    await _context.SaveChangesAsync();
        //    return Page();
        //    return RedirectToPage("./Details");
        //}
    }
}
