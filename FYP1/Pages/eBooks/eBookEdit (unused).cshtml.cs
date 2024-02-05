using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FYP1.Data;
using FYP1.Models;
using System.Composition;

namespace FYP1.Pages.eBooks
{
    public class EditModel : PageModel
    {
        private readonly FYP1.Data.ApplicationDbContext _context;

        public EditModel(FYP1.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public eBook eBook { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
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
            eBook = ebook;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(eBook).State = EntityState.Modified;

            //Fetch current department from DB.
            //ConcurrencyToken may have changed.

            var UpdateReport = await _context.eBook
                .FirstOrDefaultAsync(m => m.bookID == eBook.bookID);

            if (UpdateReport == null)
            {
                return HandleDeleted("Report");
            }

            // Set ConcurrencyToken to value read in OnGetAsync
            _context.Entry(UpdateReport).Property(p => p.ConcurrencyToken)
                .OriginalValue = eBook.ConcurrencyToken;

            if (await TryUpdateModelAsync<eBook>(
                eBook, //vairable intend to update
                "Report",
                s => s.bookStatus, s => s.title))
            {


                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (eBook)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save. " +
                            "The report was deleted by another user.");
                        return Page();
                    }

                    var dbValues = (eBook)databaseEntry.ToObject();
                    await SetDbErrorMessage(dbValues, clientValues, _context);

                    // Save the current ConcurrencyToken so next postback
                    // matches unless an new concurrency issue happens.
                    eBook.ConcurrencyToken = (byte[])dbValues.ConcurrencyToken;
                    // Clear the model error for the next postback.
                    ModelState.Remove($"{nameof(eBook)}.{nameof(eBook.ConcurrencyToken)}");

                    /*                    if (!eBookExists(eBook.bookID))
                                        {
                                            return NotFound();
                                        }
                                        else
                                        {
                                            throw;
                                        }*/
                }
            }

            return RedirectToPage("./Index");
        }

        private bool eBookExists(int id)
        {
            return (_context.eBook?.Any(e => e.bookID == id)).GetValueOrDefault();
        }

        private IActionResult HandleDeleted(string type)
        {
            // ModelState contains the posted data because of the deletion error
            // and overides the Department instance values when displaying Page().
            ModelState.AddModelError(string.Empty,
                "Unable to save. The " + type + " was deleted by another user.");
            //InstructorNameSL = new SelectList(_context.Instructors, "ID", "FullName", Department.InstructorID);
            return Page();
        }

        private async Task SetDbErrorMessage(eBook dbValues,
        eBook clientValues, ApplicationDbContext context)
        {

            /*            if (dbValues.Name != clientValues.Name)
                        {
                            ModelState.AddModelError("Department.Name",
                                $"Current value: {dbValues.Name}");
                        }
                        if (dbValues.Budget != clientValues.Budget)
                        {
                            ModelState.AddModelError("Department.Budget",
                                $"Current value: {dbValues.Budget:c}");
                        }
                        if (dbValues.StartDate != clientValues.StartDate)
                        {
                            ModelState.AddModelError("Department.StartDate",
                                $"Current value: {dbValues.StartDate:d}");
                        }
                        if (dbValues.InstructorID != clientValues.InstructorID)
                        {
                            Instructor dbInstructor = await _context.Instructors
                               .FindAsync(dbValues.InstructorID);
                            ModelState.AddModelError("Department.InstructorID",
                                $"Current value: {dbInstructor?.FullName}");
                        }*/

            ModelState.AddModelError(string.Empty,
                "The record you attempted to edit "
              + "was modified by another user after you. The "
              + "edit operation was canceled and the current values in the database "
              + "have been displayed. If you still want to edit this record, click "
              + "the Save button again.");
        }
    }
}
