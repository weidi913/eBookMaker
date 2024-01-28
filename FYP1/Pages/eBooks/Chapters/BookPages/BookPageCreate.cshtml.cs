using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FYP1.Data;
using FYP1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using FYP1.Authorization;

namespace FYP1.Pages.BookPages
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
        ViewData["chapterID"] = new SelectList(_context.Chapter, "chapterID", "chapterName");
            return Page();
        }

        [BindProperty]
        public BookPage BookPage { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.BookPage == null || BookPage == null)
            {
                return Page();
            }

            _context.BookPage.Add(BookPage);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
