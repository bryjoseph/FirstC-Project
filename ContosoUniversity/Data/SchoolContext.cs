using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Models
{
    public class SchoolContext : DbContext
    {
        public SchoolContext (DbContextOptions<SchoolContext> options) : base(options)
        {
        }

        public DbSet<ContosoUniversity.Models.Student> Student { get; set; }
        public DbSet<ContosoUniversity.Models.Enrollment> Enrollment { get; set; }
        public DbSet<ContosoUniversity.Models.Course> Courses { get; set; }
        public DbSet<ContosoUniversity.Models.Department> Departments { get; set; }
        public DbSet<ContosoUniversity.Models.Instructor> Instructors { get; set; }
        public DbSet<ContosoUniversity.Models.OfficeAssignment> OfficeAssignments { get; set; }
        public DbSet<ContosoUniversity.Models.CourseAssignment> CourseAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>().ToTable("Course");
            modelBuilder.Entity<Enrollment>().ToTable("Enrollment");
            modelBuilder.Entity<Student>().ToTable("Student");
            modelBuilder.Entity<Department>().ToTable("Department");
            modelBuilder.Entity<Instructor>().ToTable("Instructor");
            modelBuilder.Entity<OfficeAssignment>().ToTable("OfficeAssignment");
            modelBuilder.Entity<CourseAssignment>().ToTable("CourseAssignment");

            // this code establishes the composite PK because the class uses
            // two FKs as it's PK
            modelBuilder.Entity<CourseAssignment>().HasKey(c => new { c.CourseID, c.InstructorID });
        }
    }
}
