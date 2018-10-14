using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ContosoUniversity.Models;

namespace ContosoUniversity.Pages.Courses
{
    public class DepartmentNamePageModel : PageModel
    {
        public SelectList DepartmentNameSL { get; set; }

        public void PopulateDepartmentDropDownList(SchoolContext _context,
            object selectedDepartment = null)
        {
            var departmentQuery = from d in _context.Departments
                                  orderby d.Name // sorting by name
                                  select d;

            DepartmentNameSL = new SelectList(departmentQuery.AsNoTracking(),
                                                "DepartmentID", "Name", selectedDepartment);
        }
    }
}
