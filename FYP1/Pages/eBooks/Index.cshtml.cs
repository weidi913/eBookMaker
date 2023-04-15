using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FYP1.Data;
using FYP1.Models;

namespace FYP1.Pages.eBooks
{
    public class IndexModel : PageModel
    {
        private readonly FYP1.Data.ApplicationDbContext _context;

        public IndexModel(FYP1.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<eBook> eBook { get;set; } = default!;

        [BindProperty]
        public eBook eBookAdd { get; set; } = default!;


        public async Task OnGetAsync()
        {
            if (_context.eBook != null)
            {
                eBook = await _context.eBook.ToListAsync();
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


            _context.eBook.Add(eBookAdd);
            await _context.SaveChangesAsync();

/*            return RedirectToAction("Details", "OnGetAsync", new { id = eBookAdd.bookID });
*/            //rmb to modify it to better one

            return RedirectToPage("./Details", new { id = eBookAdd.bookID });
        }
    }
}
