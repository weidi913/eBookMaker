﻿using System;
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
        public string CurrentDisplay { get; set; }
        public string CurrentSearch { get; set; }
        public string CurrentTypeFilter { get; set; }
        public int CurrentDateFilter { get; set; }
        public string CurrentSort { get; set; }

        [BindProperty]
        public eBook eBookAdd { get; set; } = default!;

        public Member curUser { get; set; } = default;

        public enum BookType
        {
            Novel,
            Lecture,
            Document
        }
/*        public enum DateType
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
      
        }*/

        public string[] bookTypes = Enum.GetNames(typeof(BookType)); // Get names of enum values
/*        public string[] dateTypes = Enum.GetNames(typeof(DateType)); // Get names of enum values
        public string[] sortTypes = Enum.GetNames(typeof(SortType)); // Get names of enum values*/
        public string defaultBookType = BookType.Novel.ToString(); // Set default book type

        public async Task OnGetAsync(string? displayTab, string? searchString, string? sortOrder, 
            string? typeFilter, int? dateFilter, int? pageIndex)
        {
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


                // search string
                if (!String.IsNullOrEmpty(searchString))
                {
                    myBookQuery = myBookQuery.Where(e => e.title.Contains(searchString));
                    sharedBookQuery = sharedBookQuery.Where(b => b.title.Contains(searchString));
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
                        break;
                    case "name_des":
                        myBookQuery = myBookQuery.OrderByDescending(b => b.title);
                        sharedBookQuery = sharedBookQuery.OrderByDescending(b => b.title);
                        break;
                    case "date_asc":
                        myBookQuery = myBookQuery.OrderBy(b => b.LastUpdate);
                        sharedBookQuery = sharedBookQuery.OrderBy(b => b.LastUpdate);
                        break;
                    case "date_dsc":
                        myBookQuery = myBookQuery.OrderByDescending(b => b.LastUpdate);
                        sharedBookQuery = sharedBookQuery.OrderByDescending(b => b.LastUpdate);
                        break;
                    default:
                        myBookQuery = myBookQuery.OrderByDescending(b => b.LastUpdate);
                        sharedBookQuery = sharedBookQuery.OrderByDescending(b => b.LastUpdate);
                        sortOrder = "date_dsc";
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
                        break;
                    case "Shared":
                        myBook = await PaginatedList<eBook>.CreateAsync(
                                    myBookQuery.AsNoTracking(), 1, pageSize);
                        sharedBook = await PaginatedList<eBook>.CreateAsync(
                                    sharedBookQuery.AsNoTracking(), pageIndex ?? 1, pageSize);
                        break;
                    default:
                        myBook = await PaginatedList<eBook>.CreateAsync(
                                    myBookQuery.AsNoTracking(), 1, pageSize);
                        sharedBook = await PaginatedList<eBook>.CreateAsync(
                                    sharedBookQuery.AsNoTracking(), 1, pageSize);
                        break;
                }




            }


            CurrentDisplay = displayTab;
            CurrentSearch = searchString;
            CurrentTypeFilter = typeFilter;
            CurrentSort = sortOrder;
            CurrentDateFilter = dateFilter ?? 0;





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

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                        User, eBookAdd.authorID,
                                                        Operations.Create);
            if (!isAuthorized.Succeeded)
            {
/*                return Forbid();
*/                return Challenge();
            }

            _context.eBook.Add(eBookAdd);
            await _context.SaveChangesAsync();

/*            return RedirectToAction("Details", "OnGetAsync", new { id = eBookAdd.bookID });
*/            //rmb to modify it to better one

            return RedirectToPage("./Design", new { id = eBookAdd.bookID });
        }
    }
}