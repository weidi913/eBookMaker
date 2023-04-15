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
using System.Net;

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
                .Include(c => c.book)
                .Where(c=>c.book.bookID == eBook.bookID).ToListAsync();
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
        public async Task<IActionResult> OnPostDeleteChapterAsync(int? id, int bookid)
        {
            
            if (id == null || _context.Chapter == null)
            {
                return NotFound();
            }
            var chapter = await _context.Chapter.FindAsync(id);


            if (chapter != null)
            {
                /*                Chapter = chapter;
                                _context.Chapter.Remove(Chapter);*/

                _context.Chapter.Remove(chapter);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("OnGetAsync", new { id = bookid }); ;
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

            return RedirectToAction("OnGetAsync", new { id = eBook.bookID });
        }

        public async Task<IActionResult> OnPostDeleteBookPageAsync(int? id , int? bookID)
        {
            if (id == null || _context.Page == null)
            {
                return NotFound();
            }
            var bookpage = await _context.Page.FindAsync(id);

            if (bookpage != null)
            {
               /* BookPage = bookpage;*/
                _context.Page.Remove(bookpage);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("OnGetAsync", new { id = bookID });
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
            return RedirectToAction("OnGetAsync", new { id = eBook.bookID });
/*            return RedirectToPage("./Index");*//**/
        }

        [BindProperty]
        public String elementEditText { get; set; }="";
        [BindProperty]
        public int elementEditID { get; set; } = -1;
        [BindProperty]
        public String elementEditStyle { get; set; }

        public async Task<IActionResult> OnPostEditElementAsync()
        {
/*            if (!ModelState.IsValid)
            {
                return Page();
            }*/

            Element elementEdit = await _context.Element.FirstOrDefaultAsync(m => m.elementID == elementEditID);
            elementEdit.text = elementEditText;
            elementEdit.elementStyle = elementEditStyle;
            _context.Attach(elementEdit).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
/*                if (!ElementExists(Element.elementID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }*/
            }

            return RedirectToAction("OnGetAsync", new { id = eBook.bookID });
        }

        public async Task<IActionResult> OnPostDeleteElementAsync(/*int? id*/)
        {
/*            if (id == null || _context.Element == null)
            {
                return NotFound();
            }*/
            var element = await _context.Element.FindAsync(elementEditID);

            if (element != null)
            {
                /*                Element = element;
                                _context.Element.Remove(Element);*/

                _context.Element.Remove(element);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("OnGetAsync", new { id = eBook.bookID });
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
