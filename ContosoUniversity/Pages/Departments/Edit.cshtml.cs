using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;

namespace ContosoUniversity.Pages.Departments
{
    public class EditModel : PageModel
    {
        private readonly ContosoUniversity.Models.SchoolContext _context;

        public EditModel(ContosoUniversity.Models.SchoolContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Department Department { get; set; }
        // Replacing InstructorID with the dropdown name list
        public SelectList InstructorNameSL { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            //Department = await _context.Departments
            //    .Include(d => d.Administrator).FirstOrDefaultAsync(m => m.DepartmentID == id);

            Department = await _context.Departments
                .Include(d => d.Administrator) // eager loading code here
                .AsNoTracking()                 // tracking not required
                .FirstOrDefaultAsync(m => m.DepartmentID == id);

            if(Department == null)
            {
                return NotFound();
            }

            // For the InstructorSL list this uses strongly typed data not ViewData
            InstructorNameSL = new SelectList(_context.Instructors, "ID", "FirstMidName");
            return Page();

            //ViewData["InstructorID"] = new SelectList(_context.Instructors, "ID", "FirstMidName");
            //return Page();
            
            //if (id == null)
            //{
            //    return NotFound();
            //
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //_context.Attach(Department).State = EntityState.Modified;
            var departmentToUpdate = await _context.Departments
                .Include(a => a.Administrator)
                .FirstOrDefaultAsync(d => d.DepartmentID == id);

            // if there isn't a matching departmentID that means the dept was deleted by someone
            // else
            if (departmentToUpdate == null)
            {
                return await HandleDeletedDepartment();
            }

            // Update the RowVersion to the value when this entity was
            // fetched. If the entity has been updated after it was
            // fetched, RowVersion won't match the DB RowVersion and
            // a DbUpdateConcurrencyException is thrown.
            // A second postback will make them match, unless a new 
            // concurrency issue happens.
            _context.Entry(departmentToUpdate)
                .Property("RowVersion").OriginalValue = Department.RowVersion;

            if (await TryUpdateModelAsync<Department>(
                departmentToUpdate,
                "Department",
                s => s.Name, s => s.StartDate, s => s.Budget, s => s.InstructorID))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToPage("./Index");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    // put the exception into a variable
                    var exceptionEntry = ex.Entries.Single();
                    // get the client exception
                    var clientValues = (Department)exceptionEntry.Entity;
                    // get the database exception
                    var databaseEntry = exceptionEntry.GetDatabaseValues();

                    // if the database does not return a value from the table (another user
                    // deleted the entry)
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save. " +
                        "The department was deleted by another user.");
                        return Page();
                    }

                    var dbValues = (Department)databaseEntry.ToObject();
                    await setDbErrorMessage(dbValues, clientValues, _context);

                    // Save the current RowVersion so next postback
                    // matches unless a new concurrency issue happens.
                    Department.RowVersion = (byte[])dbValues.RowVersion;
                    // Must clear the model error for the next postback.
                    ModelState.Remove("Department.RowVersion");
                }
            }

            InstructorNameSL = new SelectList(_context.Instructors, "ID", "FullName",
                                            departmentToUpdate.InstructorID);
            return Page();
        }

        // create the HandleDeletedDepartment method
        private async Task<IActionResult> HandleDeletedDepartment()
        {
            // ModelState contains the posted data because of the deletion 
            // error and will overide the Department instance values when displaying Page().
            Department department = new Department();

            ModelState.AddModelError(string.Empty, "Unable to save." +
                                    "The department was deleted by another user.");

            InstructorNameSL = new SelectList(_context.Instructors, "ID", "FullName",
                                            Department.InstructorID);
            return Page();
        }

        private async Task setDbErrorMessage(Department dbValues, Department clientValues,
                                             SchoolContext context)
        {   
            // check the name from the client error vs. the db error
            if(dbValues.Name != clientValues.Name)
            {
                ModelState.AddModelError("Department.Name", $"CurrentValue: {dbValues.Name}");
            }
            // check the budget from the client error vs. the db error
            if(dbValues.Budget != clientValues.Budget)
            {
                ModelState.AddModelError("Department.Budget", $"CurrentValue: {dbValues.Budget}");
            }
            // check the start date from the client error vs. the db error
            if (dbValues.StartDate != clientValues.StartDate)
            {
                ModelState.AddModelError("Department.StartDate", $"CurrentValue: {dbValues.StartDate}");
            }
            // check the Instructor from the client error vs. the db error
            if (dbValues.InstructorID != clientValues.InstructorID)
            {
                // must first provide the InstructorIDs used in the SelectList
                Instructor dbInstructor = await _context.Instructors
                    .FindAsync(dbValues.InstructorID);
                ModelState.AddModelError("Department.InstructorID",
                                        $"CurrentValue: {dbInstructor?.FullName}");
            }

            ModelState.AddModelError(string.Empty,
                "The record you attempted to edit "
              + "was modified by another user after you. The "
              + "edit operation was canceled and the current values in the database "
              + "have been displayed. If you still want to edit this record, click "
              + "the Save button again.");
        }
    }
}
