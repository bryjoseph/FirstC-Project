using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    public class Student
    {
        public int ID { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")] // changing the way the column is displayed in the app
        public string LastName { get; set; }
        [Required] // making the field required in the form
        [StringLength(50, ErrorMessage = "First Name cannot be longer than 50 characters")]
        [Column("FirstName")] // changing the column name in the class && the db
        [Display(Name = "First Name")]
        public string FirstMidName { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Enrollment Date")]
        public DateTime EnrollmentDate { get; set; }
        [Display(Name = "Full Name")]
        public string FullName // adding a whole new computed column (note) only a get
        {
            get // only a get
            {
                return LastName + ", " + FirstMidName;
            }
        } // there is no setter because this column is not being added to the database

        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
