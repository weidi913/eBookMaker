using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FYP1.Data;
using FYP1.Models;

namespace FYP1.Pages.Elements
{
    public class DetailsModel : PageModel
    {
        private readonly FYP1.Data.ApplicationDbContext _context;

        public DetailsModel(FYP1.Data.ApplicationDbContext context)
        {
            _context = context;
        }

      public Element Element { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Element == null)
            {
                return NotFound();
            }

            var element = await _context.Element.FirstOrDefaultAsync(m => m.elementID == id);
            if (element == null)
            {
                return NotFound();
            }
            else 
            {
                Element = element;
            }
            return Page();
        }
    }
}
