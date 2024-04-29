// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FYP1.Authorization;
using FYP1.Data;
using FYP1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static System.Net.Mime.MediaTypeNames;

namespace FYP1.Pages.eBooks
{
    [Authorize]
    public class FileSettingModel : DI_BasePageModel
    {
        // Store the data
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<Member> _userManager;

        // Retrieve the data
        public FileSettingModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<Member> userManager)
            : base(context, authorizationService, userManager)
        {
            _context = context;
            _authorizationService = authorizationService;
            _userManager = userManager;
        }



        [BindProperty]
        public InputFileSettingModel FileSettingInput { get; set; }
        [BindProperty]
        public InputCollaborationModel CollaborationInput { get; set; }
        [BindProperty]
        public InputVersionModel VersionInput { get; set; }
        [BindProperty]
        public int bookID { get; set; }
        public Member curUser { get; set; }
        public Member authorInfo { get; set; }
        public IList<Member> memberList { get; set; }
        public IList<FYP1.Models.Version> versionList { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }
        public string FileMessage { get; set; }
        public string CollabMessage { get; set; }
        public string VersionMessage { get; set; }
        public string PublishMessage { get; set; }
        public eBook curBook { get; set; }
        public class InputVersionModel
        {
            public int versionID { get; set; }
            public string verName { get; set; }
            public int bookID { get; set; }
        }
        public class InputCollaborationModel
        {
            public int bookID { get; set; }
            public string authorID { get; set; }
        }
        public class InputFileSettingModel
        {
            public int bookID { get; set; }
            public string title { get; set; }
            public string type { get; set; }
            public string description { get; set; }
            public string edition { get; set; } 
            public string bookStatus { get; set; }
        }

        public enum BookType
        {
            [Description("Novel")]
            Novel,
            [Description("Lecture Slides")]
            Lecture,
            [Description("Document")]
            Document,
            [Description("Powerpoint Slides")]
            PowerpointSlides,
            [Description("Cookbook")]
            Cookbook,
            [Description("Travelogue")]
            Travelogue,
            [Description("Guidebook")]
            Guidebook,
            [Description("Textbook")]
            Textbook,
            [Description("Magazine")]
            Magazine,
            [Description("Comic Book")]
            Comic,
            [Description("Storybook")]
            Storybook,
            [Description("Annual Report")]
            AnnualReport,
            [Description("Product Manual")]
            ProductManual,



        }
        private string GetEnumDescription(Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var attribute = (DescriptionAttribute)fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault();
            return attribute != null ? attribute.Description : value.ToString();
        }

        public string[] bookTypes;
        public async Task<IActionResult> OnGetAsync(int id, string? FileMessage, string? CollabMessage, string? VersionMessage, string PublishMessage)
        {
            bookTypes = Enum.GetValues(typeof(BookType))
            .Cast<BookType>()
            .Select(e => GetEnumDescription(e))
            .ToArray();
            // Store the current userid
            var currentUserId = UserManager.GetUserName(User);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            curUser = user;

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
                curBook = ebook; // Store the eBook into the variable
            }
            authorInfo = await _userManager.FindByNameAsync(ebook.authorID);

            // Retrieve the collaboration if any
            var collaboration = await _context.Collaboration
                .Where(collaboration => collaboration.bookID == id)
                .Where(collaboration => collaboration.authorID == currentUserId)
                .FirstOrDefaultAsync();

            // Admin role can access
            //var isAuthorized = User.IsInRole(Constants.AdminRole);
            var isAuthorized = false;

            // Ensure the user is authorized to watch this document
            if (collaboration != null)
            {
            }
            else if (currentUserId != ebook.authorID && !isAuthorized)
            {
                return Forbid();
            }

            // Find all collaboration records for the specified book ID
            var collaborations = await _context.Collaboration
                .Where(c => c.bookID == curBook.bookID)
                .ToListAsync();



            // Extract unique authorIDs from collaboration records
            var authorIDs = collaborations.Select(c => c.authorID).Distinct();

            // Find users corresponding to the authorIDs
            memberList = new List<Member>();
            foreach (var authorID in authorIDs)
            {
                var userCollab = await _userManager.FindByNameAsync(authorID);
                if (userCollab != null)
                {
                    memberList.Add(userCollab);
                }
            }
            // memberList now contains a list of users who have collaborated on the specified book

            versionList = await _context.Version.Where(v => v.bookID == id)
                .OrderByDescending(v=>v.versionDate)
                .ToListAsync();

            this.FileMessage = FileMessage;
            this.CollabMessage = CollabMessage;
            this.VersionMessage = VersionMessage;
            this.PublishMessage = PublishMessage;

            return Page();
        }
        public async Task<IActionResult> OnPostBookPublish(int id)
        {
            var currentUserId = UserManager.GetUserName(User);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Retrieve the relevant eBook
            var ebook = await _context.eBook
            .Include(e => e.Chapters.OrderBy(c => c.chapterNo)) // Eager load chapters
                .ThenInclude(c => c.BookPages.OrderBy(c => c.pageNo)) // Eager load pages within each chapter
                    .ThenInclude(p => p.Elements.OrderBy(c => c.z_index)) // Eager load elements within each page
            .FirstOrDefaultAsync(m => m.bookID == id);
            if (ebook == null)
            {
                return NotFound(); // Book non-existent return error
            }

            // Retrieve the collaboration if any
            var collaboration = await _context.Collaboration
                .Where(collaboration => collaboration.bookID == VersionInput.bookID)
                .Where(collaboration => collaboration.authorID == currentUserId)
                .FirstOrDefaultAsync();

            // Admin role can access
            var isAuthorized = User.IsInRole(Constants.AdminRole);

            // Ensure the user is authorized to watch this document
            if (collaboration != null)
            {
            }
            else if (currentUserId != ebook.authorID && !isAuthorized)
            {
                return Forbid();
            }

            switch (ebook.bookStatus)
            {
                // published, draft, rejected, reviewed
                case "DRAFT":
                    ebook.bookStatus = "REVIEWED";
                    break;
                case "REJECTED":
                    ebook.bookStatus = "REVIEWED";
                    break;
                case "PUBLISHED":
                    ebook.bookStatus = "DRAFT";
                    break;
                default:
                    break;
            }

            if (ebook.bookStatus == "REVIEWED")
            {
                ebook.bookContent = "";

                ebook.bookContent += "<head>";
                ebook.bookContent += "<style>";
                ebook.bookContent += "p { margin:0;}";
                ebook.bookContent += "body { margin:0px; background-color:#EBECF0; display:flex; flex-direction:column; align-items:center; }";
                ebook.bookContent += ".page {margin:auto;margin-bottom:20px; background-color:white; overflow:hidden; page-break-after: always; position:relative; width:" + ebook.width + "mm; height:" + ebook.height + "mm; }";
                ebook.bookContent += ".header { color:black; background-color:white; overflow:hidden; page-break-after: always; position:relative; width:100%; height:20mm; align-items:center; display:flex; font-size:10px; padding:16px; padding-left:96px; box-sizing:border-box; }";
                ebook.bookContent += ".content { background-color:white; overflow:hidden; page-break-after: always; position:relative; width:100%; height:" + (ebook.height - 40) + "mm; }";
                ebook.bookContent += ".footer { font-size:10px; color:black; background-color:white; overflow:hidden; page-break-after: always; display:flex; justify-content:center; align-items:center; position:relative; width:100%; height:20mm; text-align:center; }";
                ebook.bookContent += ".ql-editor { position:relative; box-sizing:border-box; line-height: 1.42; height: 100%; outline: none; padding: 12px 15px; tab-size: 4; -moz-tab-size: 4; text-align: left; white-space: pre-wrap; word-wrap: break-word; font-family: Helvetica, Arial, sans-serif; font-size: 13px; }";

                ebook.bookContent += "</style>";
                ebook.bookContent += "</head>";

                ebook.bookContent += "<div class='version-history-zoom-container'>";
                ebook.bookContent += "<div style='width:100%;height:100%'>";

                var chapterNumber = 0;
                var pageNumber = 0;
                foreach (var chapter in ebook.Chapters)
                {
                    chapterNumber++;
                    foreach (var page in chapter.BookPages)
                    {
                        pageNumber++;

                        ebook.bookContent += "<div class=\"page\">";
                        ebook.bookContent += "<div class=\"header\">Chapter " + chapterNumber + " " + chapter.chapterName + "</div>";

                        // Define a regular expression to match background-image URLs
                        Regex regex = new Regex(@"background-image: url\(""([^""]+)""\)");
                        // Replace double quotes with single quotes in background-image URLs
                        string formattedHtmlContent = regex.Replace(page.backgroundStyle, "background-image: url('$1')");

                        ebook.bookContent += "<div class=\"content\" style=\"" + formattedHtmlContent + "\">";

                        foreach (var element in page.Elements)
                        {
                            ebook.bookContent += "<div class='" + element.elementType + "' style='" + element.elementStyle + "'>";
                            ebook.bookContent += element.text;
                            ebook.bookContent += "</div>";
                        }

                        ebook.bookContent += "</div>";
                        ebook.bookContent += "<div class=\"footer\">" + pageNumber + "</div>";
                        ebook.bookContent += "</div>";


                    }
                }
                ebook.bookContent += "</div>";
                ebook.bookContent += "</div>";
            }

            ebook.LastUpdate = DateTime.Now;

            _context.Attach(ebook).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("./FileSetting", new { id = ebook.bookID, PublishMessage = 1 });
            }
            catch (DbUpdateConcurrencyException error)
            {
                return RedirectToPage("./FileSetting", new { id = ebook.bookID, PublishMessage = error.ToString() });
            }
        }
        public async Task<IActionResult> OnPostFileSettingAsync()
        {
            var testingData = FileSettingInput;

            var currentUserId = UserManager.GetUserName(User);
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Retrieve the collaboration if any
            var collaboration = await _context.Collaboration
                .Where(collaboration => collaboration.bookID == FileSettingInput.bookID)
                .Where(collaboration => collaboration.authorID == currentUserId)
                .FirstOrDefaultAsync();

            // Admin role can access
            var isAuthorized = User.IsInRole(Constants.AdminRole);

            var ebook = await _context.eBook.FirstOrDefaultAsync(m => m.bookID == FileSettingInput.bookID);

            if (ebook == null)
            {
                return NotFound(); // Book non-existent return error
            }

            // Ensure the user is authorized to watch this document
            if (collaboration != null)
            {
            }
            else if (currentUserId != ebook.authorID && !isAuthorized)
            {
                return Forbid();
            }

            // Check if FirstName, BirthdayDate, and Gender are not empty or null
            if (!string.IsNullOrEmpty(FileSettingInput.title))
            {
                ebook.title = FileSettingInput.title;
            }
            // Check if FirstName, BirthdayDate, and Gender are not empty or null
            if (!string.IsNullOrEmpty(FileSettingInput.type))
            {
                ebook.type = FileSettingInput.type;
            }
            // Check if FirstName, BirthdayDate, and Gender are not empty or null
            if (!string.IsNullOrEmpty(FileSettingInput.description))
            {
                ebook.description = FileSettingInput.description;
            }

            ebook.LastUpdate = DateTime.Now;

            _context.Attach(ebook).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("./FileSetting", new { id = FileSettingInput.bookID, FileMessage = 0 });
            }
            catch (DbUpdateConcurrencyException error)
            {
                return RedirectToPage("./FileSetting", new { id = FileSettingInput.bookID, FileMessage = error.ToString() });
            }
        }

        public async Task<IActionResult> OnPostCollaborationAsync()
        {
            var currentUserId = UserManager.GetUserName(User);
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }


            // Retrieve the collaboration if any
            var collaborations = await _context.Collaboration
                .Where(collaboration => collaboration.bookID == CollaborationInput.bookID)
                .ToListAsync();
            var book = await _context.eBook.FirstOrDefaultAsync(b => b.bookID == CollaborationInput.bookID && b.authorID == currentUserId);

            var authorizedUser = false;
            var validUsername = true;

            var userAdd = await _context.Member.FirstOrDefaultAsync(u => u.UserName == CollaborationInput.authorID);

            foreach (var collaboration in collaborations)
            {
                if(collaboration.authorID == currentUserId)
                {
                    authorizedUser = true;
                }
                if(collaboration.authorID == CollaborationInput.authorID)
                {
                    validUsername = false;
                }
            }

            if(!authorizedUser && book == null)
            {
                return Forbid();
            }

            if (book.authorID == CollaborationInput.authorID)
            {
                return RedirectToPage("./FileSetting", new { id = CollaborationInput.bookID, CollabMessage = "This is the author username." });
            }

            if (!validUsername)
            {
                return RedirectToPage("./FileSetting", new { id = CollaborationInput.bookID, CollabMessage = "Username had already added." });
            }

            if (userAdd == null)
            {
                return RedirectToPage("./FileSetting", new { id = CollaborationInput.bookID, CollabMessage = "Invalid username." });
            }

            var collabAdd = new Collaboration();
            collabAdd.authorID = CollaborationInput.authorID;
            collabAdd.bookID = CollaborationInput.bookID;
            _context.Collaboration.Add(collabAdd);
            await _context.SaveChangesAsync();

            return RedirectToPage("./FileSetting", new { id = CollaborationInput.bookID, CollabMessage = "0" });

        }

        public async Task<IActionResult> OnPostCollaborationDeleteAsync()
        {

            var currentUserId = UserManager.GetUserName(User);
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Retrieve the collaboration if any
            var collaboration = await _context.Collaboration
                .Where(collaboration => collaboration.bookID == CollaborationInput.bookID)
                .Where(collaboration => collaboration.authorID == currentUserId)
                .FirstOrDefaultAsync();

            // Admin role can access
            var isAuthorized = User.IsInRole(Constants.AdminRole);

            var ebook = await _context.eBook.FirstOrDefaultAsync(m => m.bookID == CollaborationInput.bookID);

            if (ebook == null)
            {
                return NotFound(); // Book non-existent return error
            }

            // Ensure the user is authorized to watch this document
            if (collaboration != null)
            {
            }
            else if (currentUserId != ebook.authorID && !isAuthorized)
            {
                return Forbid();
            }

            var collaborationDeleted = await _context.Collaboration
                .FirstOrDefaultAsync(c => c.authorID == CollaborationInput.authorID && c.bookID == CollaborationInput.bookID);
            if(collaborationDeleted == null)
            {
                return RedirectToPage("./FileSetting", new { id = CollaborationInput.bookID, CollabMessage = "Collaboration is not found" });
            }

            _context.Collaboration.Remove(collaborationDeleted);
            await _context.SaveChangesAsync();

            return RedirectToPage("./FileSetting", new { id = CollaborationInput.bookID, CollabMessage = "1" });

        }
        public async Task<IActionResult> OnPostBookDeleteAsync()
        {
            var currentUserId = UserManager.GetUserName(User);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Admin role can access
            var isAuthorized = User.IsInRole(Constants.AdminRole);

            var ebookDelete = await _context.eBook.FirstOrDefaultAsync(m => m.bookID == this.bookID);
           

            if (ebookDelete == null)
            {
                return NotFound(); // Book non-existent return error
            }
            var bookTitle = ebookDelete.title;
            // Ensure the user is authorized to watch this document
            if (currentUserId != ebookDelete.authorID && !isAuthorized)
            {
                return Forbid();
            }

            _context.eBook.Remove(ebookDelete);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Dashboard", new { bookDeleted="0",bookDeletedInfo = "The book,"+ bookTitle + " has been succesfully deleted"});
        }
        public async Task<IActionResult> OnPostVersionAsync()
        {
            var currentUserId = UserManager.GetUserName(User);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Retrieve the relevant eBook
            var ebook = await _context.eBook
            .Include(e => e.Chapters.OrderBy(c => c.chapterNo)) // Eager load chapters
                .ThenInclude(c => c.BookPages.OrderBy(c => c.pageNo)) // Eager load pages within each chapter
                    .ThenInclude(p => p.Elements.OrderBy(c => c.z_index)) // Eager load elements within each page
            .FirstOrDefaultAsync(m => m.bookID == VersionInput.bookID);
            if (ebook == null)
            {
                return NotFound(); // Book non-existent return error
            }

            // Retrieve the collaboration if any
            var collaboration = await _context.Collaboration
                .Where(collaboration => collaboration.bookID == VersionInput.bookID)
                .Where(collaboration => collaboration.authorID == currentUserId)
                .FirstOrDefaultAsync();

            // Admin role can access
            var isAuthorized = User.IsInRole(Constants.AdminRole);

            // Ensure the user is authorized to watch this document
            if (collaboration != null)
            {
            }
            else if (currentUserId != ebook.authorID && !isAuthorized)
            {
                return Forbid();
            }

            var versionAdd = new Models.Version();
            versionAdd.bookID = VersionInput.bookID;
            versionAdd.verName = VersionInput.verName;
            versionAdd.verContent = "";
            versionAdd.versionDate = DateTime.Now;

            versionAdd.verContent += "<head>";
            versionAdd.verContent += "<style>";
           // versionAdd.verContent += $"@page {{ size: {ebook.width}mm {ebook.height}mm; }}";
            versionAdd.verContent += "p { margin:0;}";
            versionAdd.verContent += "body { margin:0px; background-color:#EBECF0; display:flex; flex-direction:column; align-items:center; }";
            versionAdd.verContent += ".page {margin:auto;margin-bottom:20px; background-color:white; overflow:hidden; page-break-after: always; position:relative; width:" +ebook.width+"mm; height:"+ebook.height+"mm; }";
            versionAdd.verContent += ".header { color:black; background-color:white; overflow:hidden; page-break-after: always; position:relative; width:100%; height:20mm; align-items:center; display:flex; font-size:10px; padding:16px; padding-left:96px; box-sizing:border-box; }";
            versionAdd.verContent += ".content { background-color:white; overflow:hidden; page-break-after: always; position:relative; width:100%; height:"+(ebook.height - 40)+"mm; }";
            versionAdd.verContent += ".footer { font-size:10px; color:black; background-color:white; overflow:hidden; page-break-after: always; display:flex; justify-content:center; align-items:center; position:relative; width:100%; height:20mm; text-align:center; }";
            versionAdd.verContent += ".ql-editor { position:relative; box-sizing:border-box; line-height: 1.42; height: 100%; outline: none; padding: 12px 15px; tab-size: 4; -moz-tab-size: 4; text-align: left; white-space: pre-wrap; word-wrap: break-word; font-family: Helvetica, Arial, sans-serif; font-size: 13px; }";

            versionAdd.verContent += "</style>";
            versionAdd.verContent += "</head>";

            versionAdd.verContent += "<div class='version-history-zoom-container'>";
            versionAdd.verContent += "<div style='width:100%;height:100%'>";

            /*            versionAdd.verContent += "<div class='version-content-header'>" +
                            "<div class=''version-content-name>"+ versionAdd.verName+ "</div>"+
                            "<input type='text'/>"+
                            "<div>";*/
            var chapterNumber = 0;
            var pageNumber = 0;
            foreach (var chapter in ebook.Chapters)
            {
                chapterNumber++;
                foreach(var page in chapter.BookPages)
                {
                    pageNumber++;

                    versionAdd.verContent += "<div class=\"page\">";
                    versionAdd.verContent += "<div class=\"header\">Chapter " + chapterNumber + " " + chapter.chapterName + "</div>";

                    // Define a regular expression to match background-image URLs
                    Regex regex = new Regex(@"background-image: url\(""([^""]+)""\)");
                    // Replace double quotes with single quotes in background-image URLs
                    string formattedHtmlContent = regex.Replace(page.backgroundStyle, "background-image: url('$1')");

                    versionAdd.verContent += "<div class=\"content\" style=\"" + formattedHtmlContent + "\">";

                    foreach (var element in page.Elements)
                    {
                        versionAdd.verContent += "<div class='" + element.elementType + "' style='"+ element.elementStyle+"'>";
                        versionAdd.verContent += element.text;
                        versionAdd.verContent += "</div>";
                    }

                    versionAdd.verContent += "</div>";
                    versionAdd.verContent += "<div class=\"footer\">"+ pageNumber +"</div>";
                    versionAdd.verContent += "</div>";


                }
            }
            versionAdd.verContent += "</div>";
            versionAdd.verContent += "</div>";

            _context.Version.Add(versionAdd);
            await _context.SaveChangesAsync();


            return RedirectToPage("./FileSetting", new { id = ebook.bookID, VersionMessage = 0 });

        }
        public async Task<IActionResult> OnPostVersionUpdateAsync()
        {
            var currentUserId = UserManager.GetUserName(User);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Retrieve the relevant eBook
            var versionUpdate = await _context.Version.FirstOrDefaultAsync(m => m.versionID == VersionInput.versionID);

            if (versionUpdate == null)
            {
                return NotFound(); // Book non-existent return error
            }

            // Retrieve the relevant eBook
            var ebook = await _context.eBook.FirstOrDefaultAsync(m => m.bookID == versionUpdate.bookID);

            if (ebook == null)
            {
                return NotFound(); // Book non-existent return error
            }

            // Retrieve the collaboration if any
            var collaboration = await _context.Collaboration
                .Where(collaboration => collaboration.bookID == versionUpdate.bookID)
                .Where(collaboration => collaboration.authorID == currentUserId)
                .FirstOrDefaultAsync();

            // Admin role can access
            var isAuthorized = User.IsInRole(Constants.AdminRole);

            // Ensure the user is authorized to watch this document
            if (collaboration != null)
            {
            }
            else if (currentUserId != ebook.authorID && !isAuthorized)
            {
                return Forbid();
            }

            if (!string.IsNullOrEmpty(VersionInput.verName))
            {
                versionUpdate.verName = VersionInput.verName;
            }

            _context.Attach(versionUpdate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("./FileSetting", new { id = ebook.bookID, VersionMessage = 1 });
            }
            catch (DbUpdateConcurrencyException error)
            {
                return RedirectToPage("./FileSetting", new { id = ebook.bookID, VersionMessage = error.ToString() });
            }
        }
        public async Task<IActionResult> OnPostVersionDeleteAsync()
        {
            var currentUserId = UserManager.GetUserName(User);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var versionDelete = await _context.Version.FindAsync(VersionInput.versionID);
            if (versionDelete == null)
            {
                return NotFound(); // Book non-existent return error
            }

            // Retrieve the relevant eBook
            var ebook = await _context.eBook.FirstOrDefaultAsync(m => m.bookID == versionDelete.bookID);

            if (ebook == null)
            {
                return NotFound(); // Book non-existent return error
            }

            // Retrieve the collaboration if any
            var collaboration = await _context.Collaboration
                .Where(collaboration => collaboration.bookID == versionDelete.bookID)
                .Where(collaboration => collaboration.authorID == currentUserId)
                .FirstOrDefaultAsync();

            // Admin role can access
            var isAuthorized = User.IsInRole(Constants.AdminRole);

            // Ensure the user is authorized to watch this document
            if (collaboration != null)
            {
            }
            else if (currentUserId != ebook.authorID && !isAuthorized)
            {
                return Forbid();
            }

            _context.Version.Remove(versionDelete);
            await _context.SaveChangesAsync();
           
            return RedirectToPage("./FileSetting", new { id = ebook.bookID, VersionMessage = 2 });
        }
    }
}
