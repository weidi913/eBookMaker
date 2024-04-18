// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
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

namespace FYP1.Pages.eBooks
{
    [Authorize]
    public class PublishedContentModel : DI_BasePageModel
    {
        // Store the data
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<Member> _userManager;

        // Retrieve the data
        public PublishedContentModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<Member> userManager)
            : base(context, authorizationService, userManager)
        {
            _context = context;
            _authorizationService = authorizationService;
            _userManager = userManager;
        }

        public int bookID { get; set; }
        public eBook publishedBook { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {


            // Ensure the passed ID and dataset is not empty
            if (id == null || _context.eBook == null)
            {
                return NotFound(); // Invalid data return error
            }

            // Retrieve the relevant eBook
            var ebook = await _context.eBook.FirstOrDefaultAsync(m => m.bookID == id);


            var isAuthorized = User.IsInRole(Constants.AdminRole) || User.IsInRole(Constants.ModeratorRole) || User.IsInRole(Constants.ReviewerRole);

            if (ebook == null)
            {
                return NotFound(); // Book non-existent return error
            }
            else if (ebook.bookStatus != "PUBLISHED" && isAuthorized)
            {
                return NotFound(); // Book non-existent return error
            }
            else
            {
                publishedBook = ebook;
            }

            return Page();
        }
    }
}
