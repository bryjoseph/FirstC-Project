using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{   
    // this table is known as a Pure Join Table (PJT)
    // a PJT is a table comprised of FKs and without a payload
    public class CourseAssignment
    {
        // notice there isn't a PK defined
        public int InstructorID { get; set; }
        public int CourseID { get; set; }
        public Instructor Instructor { get; set; }
        public Course Course { get; set; }
    }
}
