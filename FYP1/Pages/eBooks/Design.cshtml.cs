using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FYP1.Data;
using FYP1.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using FYP1.Authorization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FYP1.Pages.eBooks
{
    [Authorize]
    // Enter the eBook to view and edit the details
    public class DesignModel : DI_BasePageModel
    {
        // Store the data
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<Member> _userManager;

        // Retrieve the data
        public DesignModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<Member> userManager)
            : base(context, authorizationService, userManager)
        {
            _context = context;
            _authorizationService = authorizationService;
            _userManager = userManager;
        }

        [BindProperty] //really need it?
        public eBook eBook { get; set; } = default!; // Store the ebook record
        public IList<Chapter> ChapterList { get; set; } = default!; // Store the chapter list for this ebook
        public IList<BookPage> BookPageList { get; set; } = default!; // Store the bookpage list for this ebook
        public IList<Element> ElementList { get; set; } = default!; // Store the element list for this ebook
        public Collaboration Collaboration { get; set; } = default!; // Store the collaboration

        [BindProperty]
        public Chapter ChapterAdd { get; set; } = default!; // Chapter intended to add to the database
        [BindProperty]
        public BookPage BookPageAdd { get; set; } = default!; // Bookpage intended to add to the database
        [BindProperty]
        public Element ElementAdd { get; set; } = default!; // Element intended to add to the database
        public async Task<IActionResult> OnGetTesting()
        {
            return new JsonResult(new { status = 0, message = "Successfully updated" });
        }
        public async Task<IActionResult> OnPostTesting()
        {
            return new JsonResult(new { status = 0, message = "Successfully updated" });
        }
        public async Task<IActionResult> OnPostChapterTitle(string chapterName, int chapterID)
        {

            /*            var updateBook = await _context.eBook.FirstOrDefaultAsync(m => m.bookID == eBook.bookID);
            */
            var updateChapter = await _context.eBook.FirstOrDefaultAsync(m => m.bookID == chapterID);
            if (updateChapter == null)
            {
                return new JsonResult(new { status = 1, message = "Deleted" });
            }
            /*            else if (updateBook.title != eBook.title) {
                            return new JsonResult(new { status = 2, message = "Concurrency Error" });
                        }*/
            else
            {
                updateChapter.title = chapterName;
                _context.Attach(updateChapter).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                    return new JsonResult(new { status = 0, message = "Successfully updated" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    return new JsonResult(new { status = 3, message = "Unable to update. Please try again" });
                }
            }
        }
        // Retrieve all the data related to this book
        public async Task<IActionResult> OnPostBookTitle(string newTitleName, int bookID)
        {

            /*            var updateBook = await _context.eBook.FirstOrDefaultAsync(m => m.bookID == eBook.bookID);
            */
            var updateBook = await _context.eBook.FirstOrDefaultAsync(m => m.bookID == bookID);
            if (updateBook == null)
            {
                return new JsonResult(new { status = 1, message = "Deleted" });
            }
/*            else if (updateBook.title != eBook.title) {
                return new JsonResult(new { status = 2, message = "Concurrency Error" });
            }*/
            else { 
                updateBook.title = newTitleName;
                _context.Attach(updateBook).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                    return new JsonResult(new { status = 0, message = "Successfully updated" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    return new JsonResult(new { status = 3, message = "Unable to update. Please try again" });
                }
            }
        }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // Store the current userid
            var currentUserId = UserManager.GetUserName(User);
            
            // Ensure the passed ID and dataset is not empty
            if (id == null || _context.eBook == null)
            {
                return NotFound(); // Invalid data return error
            }

            // Retrieve the relevant eBook
            var ebook = await _context.eBook.FirstOrDefaultAsync(m => m.bookID == id);
            if (ebook == null)
            {
                return NotFound(); // Book non-existent return error
            }
            else
            {
                eBook = ebook; // Store the eBook into the variable

                // Make the preparation for future if intended to add chapter
                ChapterAdd = new Chapter(); 
                ChapterAdd.bookID = eBook.bookID;
            }

            // Retrieve the collaboration if any
            var collaboration = await _context.Collaboration
                .Where(collaboration => collaboration.bookID == id)
                .Where(collaboration => collaboration.authorID == currentUserId)
                .FirstOrDefaultAsync();

            // Admin role can access
            var isAuthorized = User.IsInRole(Constants.AdminRole);

            // Ensure the user is authorized to watch this document
            if(collaboration != null)
            {
                Collaboration = collaboration; 
            }
            else if(currentUserId != ebook.authorID && !isAuthorized)
            {
                return Forbid();
            }
            

            if (_context.Chapter != null)
            {
                // Retrieve all chapter related to this book
                ChapterList = await _context.Chapter
                .Include(chapter => chapter.book)
                .Where(chapter => chapter.book.bookID == eBook.bookID).ToListAsync();
            }

            if (_context.BookPage != null)
            {
                // Retrieve all book page related to this book
                BookPageList = await _context.BookPage
                .Include(bookpage => bookpage.Chapter).ToListAsync();
            }

            if (_context.Element != null)
            {
                // Retrieve all elements related to this book
                ElementList = await _context.Element
                .Include(element => element.BookPage).ToListAsync();
            }

            //no need to filter???? i think need to filter to avoid excessive loadinf time
            // not sure what is this maybe can delete at the finalizing stage
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
        // Add chapter async for this book
        public async Task<IActionResult> OnPostAddChapterAsync()
        {
            // Ensure it is book author
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                        User, eBook.authorID,
                                                        Operations.Create);
            // Success - Original book author
            // Fail - Not the book author
            if (!isAuthorized.Succeeded)
            {
                // Check the user is collaboration memeber 
                if (Collaboration != null)
                {
                    // Ensure that it is collaboration member
                    isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                                User, Collaboration.authorID,
                                                                Operations.Create);
                }

                // Success - It is collaboration memeber
                // Fail - it is not collaboration memeber
                if (!isAuthorized.Succeeded)
                {
                    return Forbid();
                }
            }

            if (!ModelState.IsValid || _context.Chapter == null || ChapterAdd == null)
            {
            //    return Page();
            }

            _context.Chapter.Add(ChapterAdd);
            await _context.SaveChangesAsync();

            // Return to the chapter
            return RedirectToAction("OnGetAsync", new { id = ChapterAdd.bookID }); ;
        }

        public async Task<IActionResult> OnPostDeleteChapterAsync(int? id, int bookid)
        {
            // Ensure it is book author
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                        User, eBook.authorID,
                                                        Operations.Delete);
            // Success - Original book author
            // Fail - Not the book author
            if (!isAuthorized.Succeeded)
            {
                // Check the user is collaboration memeber 
                if (Collaboration != null)
                {
                    // Ensure that it is collaboration member
                    isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                                User, Collaboration.authorID,
                                                                Operations.Delete);
                }

                // Success - It is collaboration memeber
                // Fail - it is not collaboration memeber
                if (!isAuthorized.Succeeded)
                {
                    return Forbid();
                }
            }

            if (id == null || _context.Chapter == null)
            {
                return NotFound();
            }

            var chapter = await _context.Chapter.FindAsync(id);

            if (chapter != null)
            {
                _context.Chapter.Remove(chapter);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("OnGetAsync", new { id = bookid }); ;
        }
        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAddBookPageAsync()
        {
            // Ensure it is book author
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                        User, eBook.authorID,
                                                        Operations.Create);
            // Success - Original book author
            // Fail - Not the book author
            if (!isAuthorized.Succeeded)
            {
                // Check the user is collaboration memeber 
                if (Collaboration != null)
                {
                    // Ensure that it is collaboration member
                    isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                                User, Collaboration.authorID,
                                                                Operations.Create);
                }

                // Success - It is collaboration memeber
                // Fail - it is not collaboration memeber
                if (!isAuthorized.Succeeded)
                {
                    return Forbid();
                }
            }

            if (!ModelState.IsValid || _context.BookPage == null || BookPageAdd == null)
            {
                return Page();
            }
            _context.BookPage.Add(BookPageAdd);
            await _context.SaveChangesAsync();

            return RedirectToAction("OnGetAsync", new { id = eBook.bookID });
        }

        public async Task<IActionResult> OnPostDeleteBookPageAsync(int? id , int? bookID)
        {
            // Ensure it is book author
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                        User, eBook.authorID,
                                                        Operations.Delete);
            // Success - Original book author
            // Fail - Not the book author
            if (!isAuthorized.Succeeded)
            {
                // Check the user is collaboration memeber 
                if (Collaboration != null)
                {
                    // Ensure that it is collaboration member
                    isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                                User, Collaboration.authorID,
                                                                Operations.Delete);
                }

                // Success - It is collaboration memeber
                // Fail - it is not collaboration memeber
                if (!isAuthorized.Succeeded)
                {
                    return Forbid();
                }
            }

            if (id == null || _context.BookPage == null)
            {
                return NotFound();
            }
            var bookpage = await _context.BookPage.FindAsync(id);

            if (bookpage != null)
            {
                _context.BookPage.Remove(bookpage);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("OnGetAsync", new { id = bookID });
        }


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAddElementAsync()
        {
            // Ensure it is book author
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                        User, eBook.authorID,
                                                        Operations.Create);
            // Success - Original book author
            // Fail - Not the book author
            if (!isAuthorized.Succeeded)
            {
                // Check the user is collaboration memeber 
                if (Collaboration != null)
                {
                    // Ensure that it is collaboration member
                    isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                                User, Collaboration.authorID,
                                                                Operations.Create);
                }

                // Success - It is collaboration memeber
                // Fail - it is not collaboration memeber
                if (!isAuthorized.Succeeded)
                {
                    return Forbid();
                }
            }

            if (!ModelState.IsValid || _context.Element == null || ElementAdd == null)
            {
                //return Page();
            }

            _context.Element.Add(ElementAdd);
            await _context.SaveChangesAsync();
            return RedirectToAction("OnGetAsync", new { id = eBook.bookID });
        }

        [BindProperty]
        public String elementEditText { get; set; }="";
        [BindProperty]
        public int elementEditID { get; set; } = -1;
        [BindProperty]
        public String elementEditStyle { get; set; }

        public async Task<IActionResult> OnPostEditElementAsync()
        {
            // Ensure it is book author
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                        User, eBook.authorID,
                                                        Operations.Update);
            // Success - Original book author
            // Fail - Not the book author
            if (!isAuthorized.Succeeded)
            {
                // Check the user is collaboration memeber 
                if (Collaboration != null)
                {
                    // Ensure that it is collaboration member
                    isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                                User, Collaboration.authorID,
                                                                Operations.Update);
                }

                // Success - It is collaboration memeber
                // Fail - it is not collaboration memeber
                if (!isAuthorized.Succeeded)
                {
                    return Forbid();
                }
            }

            if (!ModelState.IsValid)
            {
/*                return Page();
*/            }

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
                if (!ElementExists(elementEdit.elementID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction("OnGetAsync", new { id = eBook.bookID });
        }

        private bool ElementExists(int id)
        {
            return (_context.Element?.Any(e => e.elementID == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> OnPostDeleteElementAsync(/*int? id*/)
        {
            // Ensure it is book author
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                        User, eBook.authorID,
                                                        Operations.Delete);
            // Success - Original book author
            // Fail - Not the book author
            if (!isAuthorized.Succeeded)
            {
                // Check the user is collaboration memeber 
                if (Collaboration != null)
                {
                    // Ensure that it is collaboration member
                    isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                                User, Collaboration.authorID,
                                                                Operations.Delete);
                }

                // Success - It is collaboration memeber
                // Fail - it is not collaboration memeber
                if (!isAuthorized.Succeeded)
                {
                    return Forbid();
                }
            }

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
    }
}
