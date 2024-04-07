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
    public class VersionContentModel : DI_BasePageModel
    {
        // Store the data
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<Member> _userManager;

        // Retrieve the data
        public VersionContentModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<Member> userManager)
            : base(context, authorizationService, userManager)
        {
            _context = context;
            _authorizationService = authorizationService;
            _userManager = userManager;
        }

        public Member curUser { get; set; }
        public int bookID { get; set; }
        public FYP1.Models.Version version { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {

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

            version = await _context.Version.FirstOrDefaultAsync(v => v.versionID == id);
            if(version == null)
            {
                return NotFound(); // Book non-existent return error
            }
            // Retrieve the relevant eBook
            var ebook = await _context.eBook.FirstOrDefaultAsync(m => m.bookID == version.bookID);

            if (ebook == null)
            {
                return NotFound(); // Book non-existent return error
            }
            else
            {
                bookID = ebook.bookID;
            }

            // Retrieve the collaboration if any
            var collaboration = await _context.Collaboration
                .Where(collaboration => collaboration.bookID == id)
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

            version = await _context.Version.FirstOrDefaultAsync(v => v.versionID == id);

            return Page();
        }
    }
}
