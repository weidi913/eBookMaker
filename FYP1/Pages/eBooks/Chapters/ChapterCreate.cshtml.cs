using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FYP1.Data;
using FYP1.Models;
using FYP1.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace FYP1.Pages.Chapters
{
    public class CreateModel : DI_BasePageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<Member> _userManager;

        public CreateModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<Member> userManager)
            : base(context, authorizationService, userManager)
        {
            _context = context;
            _authorizationService = authorizationService;
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
        ViewData["bookID"] = new SelectList(_context.Set<eBook>(), "bookID", "background");
            return Page();
        }

        [BindProperty]
        public Chapter Chapter { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.Chapter == null || Chapter == null)
            {
                return Page();
            }

/*            Contact.OwnerID = UserManager.GetUserId(User);

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                        User, Contact,
                                                        Operations.Create);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }*/

            _context.Chapter.Add(Chapter);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
