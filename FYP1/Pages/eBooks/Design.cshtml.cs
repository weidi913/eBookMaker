using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FYP1.Data;
using FYP1.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using FYP1.Authorization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System;
using System.Net;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Tesseract;

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
        public static string GenerateShapeContentTemplate(string type)
        {
            string content = "<div class=\"clip-path\" style=\"";

            switch (type)
            {
                case "square":
                    content += "height:100%;width:100%; position:absolute; top:0%; bottom:0%; border: none; background:#C9D2DA;   ";
                    break;
                case "triangle":
                    content += "height:100%;width:100%; position:absolute; top:0%; bottom:0%; border: none; background:#C9D2DA; clip-path: polygon(50% 0%, 0% 100%, 100% 100%);";
                    break;
                case "circle":
                    content += "height:100%;width:100%; position:absolute; top:0%; bottom:0%; border: none; background:#C9D2DA; border-radius: 50%;";
                    break;
                case "pentagon":
                    content += "height:100%;width:100%; position:absolute; top:0%; bottom:0%; border: none; background:#C9D2DA; clip-path: polygon(50% 0%, 100% 38%, 82% 100%, 18% 100%, 0% 38%);";
                    break;
                case "parallelogram":
                    content += "height:100%;width:100%; position:absolute; top:0%; bottom:0%; border: none; background:#C9D2DA; clip-path: polygon(20% 0%, 100% 0%, 80% 100%, 0% 100%); ";
                    break;
                case "star":
                    content += "height:100%;width:100%; position:absolute; top:0%; bottom:0%; border: none; background:#C9D2DA; clip-path: polygon(50% 0%, 61% 35%, 98% 35%, 68% 57%, 79% 91%, 50% 70%, 21% 91%, 32% 57%, 2% 35%, 39% 35%); ";
                    break;
                case "hexagon":
                    content += "height:100%;width:100%; position:absolute; top:0%; bottom:0%; border: none; background:#C9D2DA; clip-path: polygon(25% 0%, 75% 0%, 100% 50%, 75% 100%, 25% 100%, 0% 50%); ";
                    break;
                case "octagon":
                    content += "height:100%;width:100%; position:absolute; top:0%; bottom:0%; border: none; background:#C9D2DA; clip-path: polygon(25% 0%, 75% 0%, 100% 25%, 100% 75%, 75% 100%, 25% 100%, 0% 75%, 0% 25%);";
                    break;
                case "diamond":
                    content += "height:100%;width:100%; position:absolute; top:0%; bottom:0%; border: none; background:#C9D2DA; clip-path: polygon(50% 0%, 100% 50%, 50% 100%, 0% 50%); ";
                    break;
                case "cross":
                    content += "height:100%;width:100%; position:absolute; top:0%; bottom:0%; border: none; background:#C9D2DA; clip-path: polygon(0% 30%, 30% 30%, 30% 0%, 70% 0%, 70% 30%, 100% 30%, 100% 70%, 70% 70%, 70% 100%, 30% 100%, 30% 70%, 0% 70%); ";
                    break;
                case "message":
                    content += "height:100%;width:100%; position:absolute; top:0%; bottom:0%; border: none; background:#C9D2DA; clip-path: polygon(0% 0%, 100% 0%, 100% 75%, 75% 75%, 75% 100%, 50% 75%, 0% 75%); ";
                    break;
                case "close":
                    content += "height:100%;width:100%; position:absolute; top:0%; bottom:0%; border: none; background:#C9D2DA; clip-path: polygon(20% 0%, 0% 20%, 30% 50%, 0% 80%, 20% 100%, 50% 70%, 80% 100%, 100% 80%, 70% 50%, 100% 20%, 80% 0%, 50% 30%); ";
                    break;
                case "left-chevron":
                    content += "height:100%;width:100%; position:absolute; top:0%; bottom:0%; border: none; background:#C9D2DA; clip-path: polygon(100% 0%, 75% 50%, 100% 100%, 25% 100%, 0% 50%, 25% 0%); ";
                    break;
                case "right-chevron":
                    content += "height:100%;width:100%; position:absolute; top:0%; bottom:0%; border: none; background:#C9D2DA; clip-path: polygon(75% 0%, 100% 50%, 75% 100%, 0% 100%, 25% 50%, 0% 0%); ";
                    break;
                case "right-arrow":
                    content += "height:100%;width:100%; position:absolute; top:0%; bottom:0%; border: none; background:#C9D2DA; clip-path: polygon(0% 20%, 60% 20%, 60% 0%, 100% 50%, 60% 100%, 60% 80%, 0% 80%); ";
                    break;
                case "left-arrow":
                    content += "height:100%;width:100%; position:absolute; top:0%; bottom:0%; border: none; background:#C9D2DA; clip-path: polygon(40% 0%, 40% 20%, 100% 20%, 100% 80%, 40% 80%, 40% 100%, 0% 50%); ";
                    break;
                case "trapezoid":
                    content += "height:100%;width:100%; position:absolute; top:0%; bottom:0%; border: none; background:#C9D2DA; clip-path: polygon(20% 0%, 80% 0%, 100% 100%, 0% 100%); ";
                    break;
                case "heart":
                    content += "height:100%;width:100%; position:absolute; top:0%; bottom:0%; border: none; background:#C9D2DA; mask: radial-gradient(at 70% 31%,#000 29%,#0000 30%),radial-gradient(at 30% 31%,#000 29%,#0000 30%),linear-gradient(#000 0 0) bottom/100% 50% no-repeat; clip-path: polygon(-41% 0,50% 91%, 141% 0); ";
                    break;
                default:
                    content += "height:100%;width:100%; position:absolute; top:0%; bottom:0%;";
                    break;
            }

            content += "\">  </div> <div class=\"editor\" style='border:none'> </div>";
            return content;
        }

        public static string GenerateElementTemplate(int elementID, string elementStyle, string type, string text)
        {
            // Construct the HTML string using C# string interpolation
            return $@"
                        <div class=""element {type}""
                            id=""{elementID}""
                            style=""{elementStyle}"">
                            {text}
                        </div>";
        }

        public static string GeneratePageTemplate(int bookPageId, int bookPageNo, float height)
        {
            return $@"
                    <div data-page-id=""{bookPageId}"" class=""design-page-content-container"">
                        <div class=""design-page-content-page-tab-list"">
                            <div class=""design-page-content-left-container"">
                                <span class=""design-page-content-page-text""><span>Page {bookPageNo}</span></span>
                            </div>
                            <div class=""design-page-content-right-container"">
                                <button type=""button""
                                        class=""design-page-content-middle-button design-page-button page-up-button"">
                                    <svg viewBox=""0 0 1024 1024"" class=""design-page-content-icon"">
                                        <path d=""M798.165 609.835l-256-256c-16.683-16.683-43.691-16.683-60.331 0l-256 256c-16.683 16.683-16.683 43.691 0 60.331s43.691 16.683 60.331 0l225.835-225.835 225.835 225.835c16.683 16.683 43.691 16.683 60.331 0s16.683-43.691 0-60.331z""></path>
                                    </svg>
                                </button>
                                <button type=""button""
                                        class=""design-page-content-middle-button design-page-button page-down-button"">
                                    <svg viewBox=""0 0 1024 1024"" class=""design-page-content-icon"">
                                        <path d=""M225.835 414.165l256 256c16.683 16.683 43.691 16.683 60.331 0l256-256c16.683-16.683 16.683-43.691 0-60.331s-43.691-16.683-60.331 0l-225.835 225.835-225.835-225.835c-16.683-16.683-43.691-16.683-60.331 0s-16.683 43.691 0 60.331z""></path>
                                    </svg>
                                </button>
                                <button type=""button""
                                        class=""design-page-content-middle-button design-page-button page-lock-toggle-button"">
                                    <svg viewBox=""0 0 1024 1024"" class=""design-page-content-icon page-lock-icon"">
                                        <path d=""M768 854v-428h-512v428h512zM380 256v86h264v-86q0-54-39-93t-93-39-93 39-39 93zM768 342q34 0 60 25t26 59v428q0 34-26 59t-60 25h-512q-34 0-60-25t-26-59v-428q0-34 26-59t60-25h42v-86q0-88 63-151t151-63 151 63 63 151v86h42zM512 726q-34 0-60-26t-26-60 26-60 60-26 60 26 26 60-26 60-60 26z""></path>
                                    </svg>
                                    <svg viewBox=""0 0 1024 1024"" class=""design-page-content-icon page-unlock-icon"">
                                        <path d=""M768 854v-428h-512v428h512zM768 342q34 0 60 25t26 59v428q0 34-26 59t-60 25h-512q-34 0-60-25t-26-59v-428q0-34 26-59t60-25h388v-86q0-54-39-93t-93-39-93 39-39 93h-82q0-88 63-151t151-63 151 63 63 151v86h42zM512 726q-34 0-60-26t-26-60 26-60 60-26 60 26 26 60-26 60-60 26z""></path>
                                    </svg>
                                </button>
                                <button type=""button""
                                        class=""design-page-content-middle-button design-page-button page-delete-button"">
                                    <svg viewBox=""0 0 1024 1024"" class=""design-page-content-icon"">
                                        <path d=""M128 320v640c0 35.2 28.8 64 64 64h576c35.2 0 64-28.8 64-64v-640h-704zM320 896h-64v-448h64v448zM448 896h-64v-448h64v448zM576 896h-64v-448h64v448zM704 896h-64v-448h64v448z""></path>
                                        <path d=""M848 128h-208v-80c0-26.4-21.6-48-48-48h-224c-26.4 0-48 21.6-48 48v80h-208c-26.4 0-48 21.6-48 48v80h832v-80c0-26.4-21.6-48-48-48zM576 128h-192v-63.198h192v63.198z""></path>
                                    </svg>
                                </button>
                                <button type=""button"" class=""design-page-button page-add-button"">
                                    <svg viewBox=""0 0 1024 1024"" class=""design-page-content-icon"">
                                        <path d=""M810 470v-86h-170v-170h-86v170h-170v86h170v170h86v-170h170zM854 86q34 0 59 25t25 59v512q0 34-25 60t-59 26h-512q-34 0-60-26t-26-60v-512q0-34 26-59t60-25h512zM170 256v598h598v84h-598q-34 0-59-25t-25-59v-598h84z""></path>
                                    </svg>
                                </button>
                            </div>
                        </div>
                        <div style='height: {height} mm' class=""design-page-content-page-content"">
                            <!-- Additional content goes here -->
                        </div>
                    </div>";
        }

        public static string GenerateChapterTemplate(Chapter chapter, int bookPageId, float height)
        {
            string chapterTemplate = $@"
                <div id=""{chapter.chapterID}"" class=""e-book-design-chapter-container""> 
                    <div class=""e-book-design-chapter-header-container"">
                        <div class=""e-book-design-chapter-horizontal-container"">
                            <span class=""e-book-design-chapter-number-text"">
                                Chapter {chapter.chapterNo}
                            </span>
                            <input value=""{chapter.chapterName}"" type=""text""
                                   data-chapter-itemid=""{chapter.chapterID}""
                                   placeholder=""Chapter Name""
                                   class=""chapter-name-input e-book-design-chapter-title-input input"" />
                            <button type=""button"" class=""chapter-toggle-button design-page-button"">
                                <svg viewBox=""0 0 1024 1024"" class=""e-book-design-chapter-icon"">
                                    <path d=""M448 576v416l-160-160-192 192-96-96 192-192-160-160zM1024 96l-192 192 160 160h-416v-416l160 160 192-192z""></path>
                                </svg>
                            </button>
                        </div>
                    </div>";

                    chapterTemplate += GeneratePageTemplate(bookPageId, 1, height);
                    chapterTemplate += @"
                </div>";

                    return chapterTemplate;
        }


        [BindProperty] //really need it?
        public eBook curBook { get; set; } = default!; // Store the ebook record
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

        public async Task<IActionResult> OnPostOCR(IFormFile postedFile)
        {
            if (postedFile != null && postedFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    postedFile.CopyTo(memoryStream);
                    memoryStream.Position = 0;

                    string tessDataPath = Path.Combine(Directory.GetCurrentDirectory(), "tessdata");
                    using (TesseractEngine engine = new TesseractEngine(tessDataPath, "eng", EngineMode.Default))
                    {
                        using (Pix pix = Pix.LoadFromMemory(memoryStream.ToArray()))
                        {
                            using (Tesseract.Page page = engine.Process(pix))
                            {
                                return new JsonResult(new { status = 0, message = "OCR function complete", result = page.GetText() });
                            }
                        }
                    }
                }
            }

            return new JsonResult(new { status = 0, message = "OCR function complete", result="hooray" });
        }
        public async Task<IActionResult> OnPostElementToggleLock(int elementID)
        {
            var element = await _context.Element.FirstOrDefaultAsync(e => e.elementID == elementID);

            if (element == null)
            {
                return new JsonResult(new { status = 1, message = "element not found" });
            }

            element.elementLock = !element.elementLock;

            // Update the database with the modified BookPage records
            _context.Element.Update(element);
            await _context.SaveChangesAsync();

            return new JsonResult(new { status = 0, message = "Element locked successfully" });
        }
        public async Task<IActionResult> OnPostBookPageToggleLock(int bookPageID)
        {
            var bookPage = await _context.BookPage.FirstOrDefaultAsync(e => e.bookPageID == bookPageID);

            if (bookPage == null)
            {
                return new JsonResult(new { status = 1, message = "book page not found" });
            }

            bookPage.pageLock = !bookPage.pageLock;

            // Update the database with the modified BookPage records
            _context.BookPage.Update(bookPage);
            await _context.SaveChangesAsync();

            return new JsonResult(new { status = 0, message = "book page locked successfully" });
        }
        public async Task<IActionResult> OnPostElementExchange(int elementID1, int elementID2)
        {


            var element1 = await _context.Element.FirstOrDefaultAsync(e => e.elementID == elementID1);
            var element2 = await _context.Element.FirstOrDefaultAsync(e => e.elementID == elementID2);
            if (element1 == null || element2 == null)
            {
                return new JsonResult(new { status = 1, message = "element not found" });
            }

            // Exchange the positions of the BookPage records
            int tempPageNo = element1.z_index;
            element1.z_index = element2.z_index;
            element2.z_index = tempPageNo;

            // Update the database with the modified BookPage records
            _context.Element.Update(element1);
            _context.Element.Update(element2);
            await _context.SaveChangesAsync();

            return new JsonResult(new { status = 0, message = "element exchanged successfully" });
        }
        public async Task<IActionResult> OnPostElementUpdate(int elementID, string? text, string? style)
        {
            var elementUpdate = await _context.Element.FindAsync(elementID);
            if(elementUpdate == null)
            {
                return new JsonResult(new { status = 1, message = "Deleted" });
            }

            if (text != null) {
                elementUpdate.text = text;
            }
            if(style != null)
            {
                elementUpdate.elementStyle = style;
            }
            _context.Attach(elementUpdate).State = EntityState.Modified;

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
        public async Task<IActionResult> OnPostElementDelete(int elementID)
        {
            var elementDelete = await _context.Element.FindAsync(elementID);

            if (elementDelete != null)
            {
                _context.Element.Remove(elementDelete);
                await _context.SaveChangesAsync();

                var remainingElements = await _context.Element
                    .Where(e => e.bookPageID == elementDelete.bookPageID && e.z_index > elementDelete.z_index)
                    .ToListAsync();

                // Update the page numbers
                foreach (var element in remainingElements)
                {
                    element.z_index -= 1;
                    _context.Element.Update(element);
                }

                await _context.SaveChangesAsync();

                return new JsonResult(new { status = 0, message = "Element successfully deleted" });
            }
            else
            {
                return new JsonResult(new { status = 1, message = "Already deleted nowhere" });
            }
        }
        public async Task<IActionResult> OnPostChartElement(int bookPageID, string chartContent)
        {
            var elementAdd = new Element();
            int elementCount = _context.Element.Count(e => e.bookPageID == bookPageID);
            elementAdd.bookPageID = bookPageID;
            elementAdd.z_index = elementCount;
            elementAdd.text = chartContent;

            elementAdd.elementType = "chart";
            elementAdd.elementStyle = "left:20px;top:20px;position:absolute; border: none; width:200px; height:200px;font-size:0px;";

            _context.Element.Add(elementAdd);
            await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                status = 0,
                message = "successful",
                elementID = elementAdd.elementID,
                bookPageID = elementAdd.bookPageID,
                htmlContent = GenerateElementTemplate(elementAdd.elementID, elementAdd.elementStyle, "chart", elementAdd.text)
            });
        }
        public async Task<IActionResult> OnPostTextElement(int bookPageID, string textContent)
        {
            var elementAdd = new Element();
            int elementCount = _context.Element.Count(e => e.bookPageID == bookPageID);
            elementAdd.bookPageID = bookPageID;
            elementAdd.z_index = elementCount;
            elementAdd.text = textContent;

            elementAdd.elementType = "text";
            elementAdd.elementStyle = "left:20px;top:20px;position:absolute; border: none; width:200px; height:200px;";

            _context.Element.Add(elementAdd);
            await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                status = 0,
                message = "successful",
                elementID = elementAdd.elementID,
                bookPageID = elementAdd.bookPageID,
                htmlContent = GenerateElementTemplate(elementAdd.elementID, elementAdd.elementStyle, "text", elementAdd.text)
            });
        }
        public async Task<IActionResult> OnPostImageElement(int bookPageID, string imageContent)
        {
            var elementAdd = new Element();
            int elementCount = _context.Element.Count(e => e.bookPageID == bookPageID);
            elementAdd.bookPageID = bookPageID;
            elementAdd.z_index = elementCount;
            elementAdd.text = "";

            elementAdd.elementType = "image";
            elementAdd.elementStyle = "left:20px;top:20px;position:absolute; border: none;";
            elementAdd.elementStyle += imageContent;

            _context.Element.Add(elementAdd);
            await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                status = 0,
                message = "successful",
                elementID = elementAdd.elementID,
                bookPageID = elementAdd.bookPageID,
                htmlContent = GenerateElementTemplate(elementAdd.elementID, elementAdd.elementStyle, "image", elementAdd.text)
            });
        }
        public async Task<IActionResult> OnPostShapeElement(int bookPageID, string elementType)
        {
            var elementAdd = new Element();
            int elementCount = _context.Element.Count(e => e.bookPageID == bookPageID);
            elementAdd.bookPageID = bookPageID;
            elementAdd.z_index = elementCount;
            elementAdd.text = GenerateShapeContentTemplate(elementType);

            elementAdd.elementType = "shape";
            elementAdd.elementStyle = "left:20px;top:20px;position:absolute;height:200px;width:200px;justify-content:center;align-items:center;border: none;";

            _context.Element.Add(elementAdd);
            await _context.SaveChangesAsync();

            return new JsonResult(new { status = 0, message="successful", elementID = elementAdd.elementID, bookPageID = elementAdd.bookPageID,
                htmlContent = GenerateElementTemplate(elementAdd.elementID,elementAdd.elementStyle, "shape", elementAdd.text)});
        }
        public async Task<IActionResult> OnPostChapterTitle(string chapterName, int chapterID)
        {

            /*            var updateBook = await _context.eBook.FirstOrDefaultAsync(m => m.bookID == eBook.bookID);
            */
            var updateChapter = await _context.Chapter.FirstOrDefaultAsync(m => m.chapterID == chapterID);
            if (updateChapter == null)
            {
                return new JsonResult(new { status = 1, message = "Deleted" });
            }
            /*            else if (updateBook.title != eBook.title) {
                            return new JsonResult(new { status = 2, message = "Concurrency Error" });
                        }*/
            else
            {
                updateChapter.chapterName = chapterName;
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
        public async Task<IActionResult> OnPostChapter(int bookID)
        {
            var chapterAdd = new Chapter();
            var eBook = await _context.eBook.FirstOrDefaultAsync(b => b.bookID == bookID);
            int chapterCount = _context.Chapter.Count(c => c.bookID == bookID);
            chapterAdd.chapterNo = chapterCount+1;
            chapterAdd.bookID = bookID;
            chapterAdd.chapterName = "";

            _context.Chapter.Add(chapterAdd);
            await _context.SaveChangesAsync();

            var bookPageAdd = new BookPage();
            bookPageAdd.chapterID = chapterAdd.chapterID;
            bookPageAdd.pageNo = 1;

            _context.BookPage.Add(bookPageAdd);
            await _context.SaveChangesAsync();

            return new JsonResult(new { status = 0,chapterID = chapterAdd.chapterID, pageID = bookPageAdd.bookPageID, 
                htmlContent = GenerateChapterTemplate(chapterAdd,bookPageAdd.bookPageID, eBook.height) });
        }

        public async Task<IActionResult> OnPostBookPage(int bookPageID)
        {
            var bookPageAdd = new BookPage();
            

            var curBookPage = await _context.BookPage.FirstOrDefaultAsync(p => p.bookPageID == bookPageID);

            if(curBookPage == null)
            {
                return new JsonResult(new { status = 1, message= "Cur Book PAge not found" });
            }
            var chapter = await _context.Chapter.FirstOrDefaultAsync(c => c.chapterID == curBookPage.chapterID);

            // Check if the chapter exists
            if (chapter == null)
            {
                return new JsonResult(new { status = 2, message = "Cur chapter not found" });
            }
            var eBook = await _context.eBook.FirstOrDefaultAsync(b => b.bookID == chapter.bookID);
            if(eBook == null)
            {
                return new JsonResult(new { status = 3, message = "Cur ebook not found" });

            }

            bookPageAdd.chapterID = curBookPage.chapterID;
            bookPageAdd.pageNo = curBookPage.pageNo +1;

            _context.BookPage.Add(bookPageAdd);
            await _context.SaveChangesAsync();

            var remainingBookPages = await _context.BookPage
                .Where(p => p.chapterID == bookPageAdd.chapterID && p.pageNo >= bookPageAdd.pageNo && p.bookPageID != bookPageAdd.bookPageID)
                .ToListAsync();

            // Update the page numbers
            foreach (var page in remainingBookPages)
            {
                page.pageNo += 1;
                _context.BookPage.Update(page);
            }

            await _context.SaveChangesAsync();

            return new JsonResult(new { status = 0, pageNo = bookPageAdd.pageNo, htmlContent = GeneratePageTemplate(bookPageAdd.bookPageID,bookPageAdd.pageNo, eBook.height) });
        }
        public async Task<IActionResult> OnPostBookPageExchange(int bookPageID1, int bookPageID2)
        {


            var BookPage1 = await _context.BookPage.FirstOrDefaultAsync(p => p.bookPageID == bookPageID1);
            var BookPage2 = await _context.BookPage.FirstOrDefaultAsync(p => p.bookPageID == bookPageID2);
            if(BookPage1 == null || BookPage2 == null)
            {
                return new JsonResult(new { status = 1, message = "Book PAge not found" });
            }

            // Exchange the positions of the BookPage records
            int tempPageNo = BookPage1.pageNo;
            BookPage1.pageNo = BookPage2.pageNo;
            BookPage2.pageNo = tempPageNo;

            // Update the database with the modified BookPage records
            _context.BookPage.Update(BookPage1);
            _context.BookPage.Update(BookPage2);
            await _context.SaveChangesAsync();

            return new JsonResult(new { status = 0, message = "Book Pages exchanged successfully" });
        }
        public async Task<IActionResult> OnPostBookPageDelete(int bookPageID)
        {
            var bookPageDelete = await _context.BookPage.FindAsync(bookPageID);

            if (bookPageDelete != null)
            {
                _context.BookPage.Remove(bookPageDelete);
                await _context.SaveChangesAsync();

                var remainingBookPages = await _context.BookPage
                    .Where(p => p.chapterID == bookPageDelete.chapterID && p.pageNo > bookPageDelete.pageNo)
                    .ToListAsync();

                // Update the page numbers
                foreach (var page in remainingBookPages)
                {
                    page.pageNo -= 1;
                    _context.BookPage.Update(page);
                }

                await _context.SaveChangesAsync();

                // Check if the chapter associated with the deleted page has any remaining pages
                var chapterID = bookPageDelete.chapterID;
                var remainingPagesCount = await _context.BookPage.CountAsync(p => p.chapterID == chapterID);
                bool deleteChapter = remainingPagesCount == 0;

                // If there are no remaining pages, delete the chapter as well
                if (deleteChapter)
                {
                    var chapterDelete = await _context.Chapter.FindAsync(chapterID);
                    if (chapterDelete != null)
                    {
                        _context.Chapter.Remove(chapterDelete);
                        await _context.SaveChangesAsync();

                        // Get the remaining chapters after deleting the chapter
                        var remainingChapters = await _context.Chapter
                            .Where(c => c.bookID == chapterDelete.bookID && c.chapterNo > chapterDelete.chapterNo)
                            .ToListAsync();

                        // Update the chapter numbers
                        foreach (var chapter in remainingChapters)
                        {
                            chapter.chapterNo -= 1;
                            _context.Chapter.Update(chapter);
                        }

                        await _context.SaveChangesAsync();
                    }
                }




                return new JsonResult(new { status = 0, deleteChapter= deleteChapter,message = "Successfully deleted" });
            }
            else
            {
                return new JsonResult(new { status = 1, message = "Already deleted nowhere" });
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
           // var ebook = await _context.eBook.FirstOrDefaultAsync(m => m.bookID == id);

            var ebook = await _context.eBook
                        .Include(e => e.Chapters.OrderBy(c=>c.chapterNo)) // Eager load chapters
                            .ThenInclude(c => c.BookPages.OrderBy(c=>c.pageNo)) // Eager load pages within each chapter
                                .ThenInclude(p => p.Elements.OrderBy(c=>c.z_index)) // Eager load elements within each page
                        .FirstOrDefaultAsync(m => m.bookID == id);
            if (ebook == null)
            {
                return NotFound(); // Book non-existent return error
            }
            else
            {
                curBook = ebook; // Store the eBook into the variable

                // Make the preparation for future if intended to add chapter
                ChapterAdd = new Chapter(); 
                ChapterAdd.bookID = curBook.bookID;
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
                .Where(chapter => chapter.book.bookID == curBook.bookID).ToListAsync();
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
                                                        User, curBook.authorID,
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
                                                        User, curBook.authorID,
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
                                                        User, curBook.authorID,
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

            return RedirectToAction("OnGetAsync", new { id = curBook.bookID });
        }

        public async Task<IActionResult> OnPostDeleteBookPageAsync(int? id , int? bookID)
        {
            // Ensure it is book author
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                        User, curBook.authorID,
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
                                                        User, curBook.authorID,
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
            return RedirectToAction("OnGetAsync", new { id = curBook.bookID });
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
                                                        User, curBook.authorID,
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

            return RedirectToAction("OnGetAsync", new { id = curBook.bookID });
        }

        private bool ElementExists(int id)
        {
            return (_context.Element?.Any(e => e.elementID == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> OnPostDeleteElementAsync(/*int? id*/)
        {
            // Ensure it is book author
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                        User, curBook.authorID,
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
            return RedirectToAction("OnGetAsync", new { id = curBook.bookID });
        }
    }
}
