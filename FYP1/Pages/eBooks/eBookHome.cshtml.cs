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

namespace FYP1.Pages.eBooks
{
    public class eBookHomeModel : DI_BasePageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<Member> _userManager;

        public eBookHomeModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<Member> userManager)
            : base(context, authorizationService, userManager)
        {
            _context = context;
            _authorizationService = authorizationService;
            _userManager = userManager;
        }

        public IList<eBook> eBook { get;set; } = default!;

        [BindProperty]
        public eBook eBookAdd { get; set; } = default!;


        public async Task OnGetAsync()
        {
            if (_context.eBook != null)
            {
                eBook = await _context.eBook
                    .Where(e => e.authorID == UserManager.GetUserId(User))
                    .ToListAsync();
            }
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

            eBookAdd.authorID = UserManager.GetUserId(User);

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                        User, eBookAdd.authorID,
                                                        Operations.Create);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
                return Challenge();
            }

            _context.eBook.Add(eBookAdd);
            await _context.SaveChangesAsync();

/*            return RedirectToAction("Details", "OnGetAsync", new { id = eBookAdd.bookID });
*/            //rmb to modify it to better one

            return RedirectToPage("./Details", new { id = eBookAdd.bookID });
        }
    }
}
