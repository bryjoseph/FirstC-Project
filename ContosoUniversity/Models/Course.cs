﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Models
{
    public class Course
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // specifies the PK is generated by the app not the DB
        [Display(Name = "Course Number")]
        public int CourseID { get; set; }
        // the reason the app generates the PK is because when the course is created the course
        // number will act as the PK, 1000, 2001, 3004, etc
        // [DatabaseGenerated(DatabaseGeneratedOption.Computed)]

        [StringLength(50, MinimumLength = 3)] // min length same as making required
        public string Title { get; set; }

        [Range(0, 5)]
        public int Credits { get; set; }

        // not required because the navigation property is set below
        // however, EF Core creates shadow properties for automatically created FKs
        // adding the DepartmentID FK here and not just leaving it to a navigation property
        // not only clarifies the relationship for migrations, but also EF Core does not need to
        // fetch the entire Department entity before populating the FK DepartmentID
        public int DepartmentID { get; set; }

        public Department Department { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<CourseAssignment> CourseAssignments { get; set; }
    }
}
