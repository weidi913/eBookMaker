using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FYP1.Data;
using FYP1.Models;
using FYP1.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace FYP1.Pages.eBooks
{
    [Authorize]
    public class DashboardModel : DI_BasePageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<Member> _userManager;
        private readonly IConfiguration Configuration;

        public DashboardModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<Member> userManager,
            IConfiguration configuration)
            : base(context, authorizationService, userManager)
        {
            _context = context;
            _authorizationService = authorizationService;
            _userManager = userManager;
            Configuration = configuration;
        }

        public PaginatedList<eBook> myBook { get;set; } = default!;
        public PaginatedList<eBook> sharedBook { get; set; } = default!;
        public IList<eBook> recentBook { get; set; } = default!;
        public PaginatedList<eBook> reviewedBook { get; set; } = default!;
        public PaginatedList<eBook> publishedBook { get; set; } = default!;
        public string CurrentDisplay { get; set; }
        public string CurrentSearch { get; set; }
        public string CurrentTypeFilter { get; set; }
        public int CurrentDateFilter { get; set; }
        public int CurrentPageIndex { get; set; }
        public string CurrentSort { get; set; }
        public string bookDeleted { get; set; }

        [BindProperty]
        public eBook eBookAdd { get; set; } = default!;
        public Member? curUser { get; set; }

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
        public string[] dateTypes;
        public string[] sortTypes;

        public enum DateType
        {
            [Description("Anytime")]
            Anytime = 0,
            [Description("Today")]
            Today = 1,
            [Description("Last 7 days")]
            Last7Days = 7,
            [Description("Last 30 days")]
            Last30Days = 30,
            [Description("Last 90 days")]
            Last90Days = 90
        }

        public enum SortType
        {
            [Description("Newest Edited")]
            date_dsc,
            [Description("Oldest Edited")]
            date_asc,
            [Description("Name A-Z")]
            name_asc,
            [Description("Name Z-A")]
            name_des,

        }

        public string defaultBookType = BookType.Novel.ToString(); // Set default book type

        public async Task OnGetAsync(string? displayTab, string? searchString, string? sortOrder, 
            string? typeFilter, int? dateFilter, int? pageIndex, string? bookDeleted, string? PublishMessage)
        {
            bookTypes = Enum.GetValues(typeof(BookType))
                        .Cast<BookType>()
                        .Select(e => GetEnumDescription(e))
                        .ToArray();

            curUser = await _userManager.GetUserAsync(User);
            
            if (curUser != null && _context.eBook != null)
            {
                recentBook = await _context.eBook
                    .Where(e => e.authorID == curUser.UserName)
                    .OrderByDescending(e => e.LastUpdate) // Assuming CreatedDate is the property indicating the creation date
                    .Take(10)
                    .ToListAsync();

                var myBookQuery = from e in _context.eBook select e;
                myBookQuery = myBookQuery.Where(e => e.authorID == curUser.UserName);

                var sharedBookQuery = (from c in _context.Collaboration
                                   join b in _context.eBook on c.bookID equals b.bookID
                                   where c.authorID == curUser.UserName
                                   select b).Distinct();

                var reviewedBookQuery = from e in _context.eBook select e;
                reviewedBookQuery = reviewedBookQuery.Where(e => e.bookStatus.Contains("REVIEWED"));

                var publishedBookQuery = from e in _context.eBook select e;
                publishedBookQuery = publishedBookQuery.Where(e => e.bookStatus.Contains("PUBLISHED"));


                // search string
                if (!String.IsNullOrEmpty(searchString))
                {
                    myBookQuery = myBookQuery.Where(e => e.title.Contains(searchString));
                    sharedBookQuery = sharedBookQuery.Where(b => b.title.Contains(searchString));
                    reviewedBookQuery = reviewedBookQuery.Where(b => b.title.Contains(searchString));
                    publishedBookQuery = publishedBookQuery.Where(b => b.title.Contains(searchString));
                }

                // type filter
                if (String.IsNullOrEmpty(typeFilter))
                {
                    typeFilter = "All";
                }
                switch(typeFilter)
                {
                    case "All":
                        break;
                    default:
                        myBookQuery = myBookQuery.Where(e => e.type == typeFilter);
                        sharedBookQuery = sharedBookQuery.Where(b => b.type == typeFilter);
                        reviewedBookQuery = reviewedBookQuery.Where(b => b.type == typeFilter);
                        publishedBookQuery = publishedBookQuery.Where(b => b.type == typeFilter);
                        break;

                }

                // date filter
                switch (dateFilter)
                {
                    case 0:
                        break;
                    case 1:
                    case 7:
                    case 30:
                    case 90:
                        // Get the current date
                        var currentDate = DateTime.Now;

                        // Calculate the start date by subtracting 30 days
                        var startDate = currentDate.AddDays((double)(-1 * dateFilter));

                        myBookQuery = myBookQuery.Where(b => b.LastUpdate >= startDate && b.LastUpdate <= currentDate);
                        sharedBookQuery = sharedBookQuery.Where(b => b.LastUpdate >= startDate && b.LastUpdate <= currentDate);
                        reviewedBookQuery = reviewedBookQuery.Where(b => b.LastUpdate >= startDate && b.LastUpdate <= currentDate);
                        publishedBookQuery = publishedBookQuery.Where(b => b.LastUpdate >= startDate && b.LastUpdate <= currentDate);
                        break;
                    default:
                        break;
                }

                // sort order
                switch (sortOrder)
                {
                    case "name_asc":
                        myBookQuery = myBookQuery.OrderBy(b => b.title);
                        sharedBookQuery = sharedBookQuery.OrderBy(b => b.title);
                        reviewedBookQuery = reviewedBookQuery.OrderBy(b => b.title);
                        publishedBookQuery = publishedBookQuery.OrderBy(b => b.title);
                        break;
                    case "name_des":
                        myBookQuery = myBookQuery.OrderByDescending(b => b.title);
                        sharedBookQuery = sharedBookQuery.OrderByDescending(b => b.title);
                        reviewedBookQuery = reviewedBookQuery.OrderByDescending(b => b.title);
                        publishedBookQuery = publishedBookQuery.OrderByDescending(b => b.title);
                        break;
                    case "date_asc":
                        myBookQuery = myBookQuery.OrderBy(b => b.LastUpdate);
                        sharedBookQuery = sharedBookQuery.OrderBy(b => b.LastUpdate);
                        reviewedBookQuery = reviewedBookQuery.OrderBy(b => b.LastUpdate);
                        publishedBookQuery = publishedBookQuery.OrderBy(b => b.LastUpdate);
                        break;
                    case "date_dsc":
                        myBookQuery = myBookQuery.OrderByDescending(b => b.LastUpdate);
                        sharedBookQuery = sharedBookQuery.OrderByDescending(b => b.LastUpdate);
                        reviewedBookQuery = reviewedBookQuery.OrderByDescending(b => b.LastUpdate);
                        publishedBookQuery = publishedBookQuery.OrderByDescending(b => b.LastUpdate);
                        break;
                    default:
                        myBookQuery = myBookQuery.OrderByDescending(b => b.LastUpdate);
                        sharedBookQuery = sharedBookQuery.OrderByDescending(b => b.LastUpdate);
                        reviewedBookQuery = reviewedBookQuery.OrderBy(b => b.LastUpdate);
                        publishedBookQuery = publishedBookQuery.OrderByDescending(b => b.LastUpdate);
                        //sortOrder = "date_dsc";
                        break;
                }


                int pageSize = Configuration.GetValue("PageSize", 20);
                
                // display tab and pagination list
                if (String.IsNullOrEmpty(displayTab))
                {
                    displayTab = "Home";
                }
                switch (displayTab)
                {
                    case "Projects":
                        myBook = await PaginatedList<eBook>.CreateAsync(
                                    myBookQuery.AsNoTracking(), pageIndex ?? 1, pageSize);
                        sharedBook = await PaginatedList<eBook>.CreateAsync(
                                    sharedBookQuery.AsNoTracking(), 1, pageSize);
                        reviewedBook = await PaginatedList<eBook>.CreateAsync(
                                    reviewedBookQuery.AsNoTracking(), 1, pageSize);
                        publishedBook = await PaginatedList<eBook>.CreateAsync(
                                    publishedBookQuery.AsNoTracking(), 1, pageSize);
                        break;
                    case "Shared":
                        myBook = await PaginatedList<eBook>.CreateAsync(
                                    myBookQuery.AsNoTracking(), 1, pageSize);
                        sharedBook = await PaginatedList<eBook>.CreateAsync(
                                    sharedBookQuery.AsNoTracking(), pageIndex ?? 1, pageSize);
                        reviewedBook = await PaginatedList<eBook>.CreateAsync(
                                    reviewedBookQuery.AsNoTracking(), 1, pageSize);
                        publishedBook = await PaginatedList<eBook>.CreateAsync(
                                    publishedBookQuery.AsNoTracking(), 1, pageSize);
                        break;
                    case "Management":
                        myBook = await PaginatedList<eBook>.CreateAsync(
                                    myBookQuery.AsNoTracking(), 1, pageSize);
                        sharedBook = await PaginatedList<eBook>.CreateAsync(
                                    sharedBookQuery.AsNoTracking(), 1, pageSize);
                        reviewedBook = await PaginatedList<eBook>.CreateAsync(
                                    reviewedBookQuery.AsNoTracking(), pageIndex ?? 1, pageSize);
                        publishedBook = await PaginatedList<eBook>.CreateAsync(
                                    publishedBookQuery.AsNoTracking(), 1, pageSize);
                        break;
                    case "Published":
                        myBook = await PaginatedList<eBook>.CreateAsync(
                                    myBookQuery.AsNoTracking(), 1, pageSize);
                        sharedBook = await PaginatedList<eBook>.CreateAsync(
                                    sharedBookQuery.AsNoTracking(), 1, pageSize);
                        reviewedBook = await PaginatedList<eBook>.CreateAsync(
                                    reviewedBookQuery.AsNoTracking(), 1, pageSize);
                        publishedBook = await PaginatedList<eBook>.CreateAsync(
                                    publishedBookQuery.AsNoTracking(), pageIndex ?? 1, pageSize);
                        break;
                    default:
                        myBook = await PaginatedList<eBook>.CreateAsync(
                                    myBookQuery.AsNoTracking(), 1, pageSize);
                        sharedBook = await PaginatedList<eBook>.CreateAsync(
                                    sharedBookQuery.AsNoTracking(), 1, pageSize);
                        reviewedBook = await PaginatedList<eBook>.CreateAsync(
                                    reviewedBookQuery.AsNoTracking(), 1, pageSize);
                        publishedBook = await PaginatedList<eBook>.CreateAsync(
                                    publishedBookQuery.AsNoTracking(), 1, pageSize);
                        break;
                }
            }

            CurrentDisplay = displayTab;
            CurrentSearch = searchString;
            CurrentTypeFilter = typeFilter;
            CurrentSort = sortOrder;
            CurrentDateFilter = dateFilter ?? 0;
            CurrentPageIndex = pageIndex ?? 0;
            this.bookDeleted = bookDeleted;
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostBookAsync()
        {
            
            if (!ModelState.IsValid || _context.eBook == null || eBookAdd == null)
            {
                return RedirectToPage("./Index");
                //Should add in something
                //to show the error
            }

            eBookAdd.authorID = UserManager.GetUserName(User);

            _context.eBook.Add(eBookAdd);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Design", new { id = eBookAdd.bookID });
        }

        public async Task<IActionResult> OnPostBookPublish()
        {
            // Retrieve the relevant eBook
            var ebook = await _context.eBook.FirstOrDefaultAsync(m => m.bookID == eBookAdd.bookID);
            if (ebook == null)
            {
                return NotFound(); // Book non-existent return error
            }

            // Admin role can access
            var isAuthorized = User.IsInRole(Constants.AdminRole) || User.IsInRole(Constants.ReviewerRole) || User.IsInRole(Constants.ModeratorRole);

            // Ensure the user is authorized to watch this document
            if (!isAuthorized)
            {
                return Forbid();
            }

            ebook.bookStatus = eBookAdd.bookStatus;
            ebook.LastUpdate = DateTime.Now;

            _context.Attach(ebook).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("./Dashboard", new { displayTab = "Management", id = ebook.bookID, bookDeleted = 1 });
            }
            catch (DbUpdateConcurrencyException error)
            {
                return RedirectToPage("./Dashboard", new { displayTab = "Management", bookDeleted = 2});
            }
        }
    }
}
