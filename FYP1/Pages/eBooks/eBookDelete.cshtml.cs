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
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;

namespace FYP1.Pages.eBooks
{
    public class DeleteModel : DI_BasePageModel
    {
        private readonly ApplicationDbContext _context;
    private readonly IAuthorizationService _authorizationService;
    private readonly UserManager<Member> _userManager;

    public DeleteModel(
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
        public eBook eBook { get; set; } = default!;
        public string ConcurrencyErrorMessage { get; set; }


        public async Task<IActionResult> OnGetAsync(int? id, bool? concurrencyError)
        {
            if (id == null || _context.eBook == null)
            {
                return NotFound();
            }

            var ebook = await _context.eBook.FirstOrDefaultAsync(m => m.bookID == id);

            if (ebook == null)
            {
                return NotFound();
            }
            else 
            {
                eBook = ebook;
            }

            if (concurrencyError.GetValueOrDefault())
            {
                ConcurrencyErrorMessage = "The record you attempted to delete "
                  + "was modified by another user after you selected delete. "
                  + "The delete operation was canceled and the current values in the "
                  + "database have been displayed. If you still want to delete this "
                  + "record, click the Delete button again.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.eBook == null)
            {
                return NotFound();
            }

            try
            {
                if (await _context.eBook.AnyAsync(
                    m => m.bookID == id))
                {
                    _context.eBook.Remove(eBook);
                    await _context.SaveChangesAsync();
                }

                var isAuthorized = await AuthorizationService.AuthorizeAsync(User, "Not important",
                    Operations.Approve);

                if (isAuthorized.Succeeded)
                {
                    return RedirectToPage("./Reports/Index");

                }
                return RedirectToPage("./myBlog");
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToPage("./Delete",
                    new { concurrencyError = true, id = id });
            }
        }

/*            if (id == null || _context.eBook == null)
            {
                return NotFound();
            }
            var ebook = await _context.eBook.FindAsync(id);

            if (ebook != null)
            {
                eBook = ebook;
                _context.eBook.Remove(eBook);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");*/
        
    }
}

