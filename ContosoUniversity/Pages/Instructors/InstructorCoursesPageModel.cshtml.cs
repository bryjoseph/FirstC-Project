using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContosoUniversity.Models;
using ContosoUniversity.Data;
using ContosoUniversity.Models.SchoolViewsModels;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContosoUniversity.Pages.Instructors
{
    public class InstructorCoursesPageModel : PageModel
    {
        public List<AssignedCourseData> AssignedCourseDataList;

        public void PopulateAssignedCourseData(SchoolContext context, Instructor instructor)
        {
            var allCourses = context.Courses;
            // create a hashSet (so they don't repeat) of the courseIDs a specific instructor teaches
            var instructorCourses = new HashSet<int>(
                instructor.CourseAssignments.Select(c => c.CourseID));
            // instantiate a new list object
            AssignedCourseDataList = new List<AssignedCourseData>();
            // for each ID in the hashSet add the course to the AssignedCourseList
            foreach(var course in allCourses)
            {
                AssignedCourseDataList.Add(new AssignedCourseData
                {
                    CourseID = course.CourseID,
                    Title = course.Title,
                    Assigned = instructorCourses.Contains(course.CourseID)
                });
            }
        }

        public void UpdateInstructorsCourses(SchoolContext context, string[] selectedCourses,
            Instructor instructorToUpdate)
        {
            // validation to check for a null value
            if (selectedCourses == null)
            {
                instructorToUpdate.CourseAssignments = new List<CourseAssignment>();
                return;
            }

            // this is creating a HS of the courses the user selected
            var selectedCoursesHS = new HashSet<string>(selectedCourses);
            // creating the hash of the courses the instructor teaches
            var instructorCourses = new HashSet<int>(
                instructorToUpdate.CourseAssignments.Select(c => c.Course.CourseID));
            // now go through each available Course
            foreach(var course in context.Courses)
            {   
                // must be ToString() because the method received a string[]
                // this route is to add a CourseAssignment
                if(selectedCoursesHS.Contains(course.CourseID.ToString()))
                {
                    // if the a courseID from the db does NOT match a courseID the instructor teaches
                    if(!instructorCourses.Contains(course.CourseID))
                    {
                        // if there isn't a match, this will now add the CourseAssignment
                        // to the instructorToUpdate
                        instructorToUpdate.CourseAssignments.Add(
                            new CourseAssignment
                            {
                                InstructorID = instructorToUpdate.ID,
                                CourseID = course.CourseID
                            });
                    }
                }
                // the else is removing a CourseAssignment
                else
                {
                    if(instructorCourses.Contains(course.CourseID))
                    {
                        CourseAssignment courseToRemove = instructorToUpdate
                            .CourseAssignments
                            .SingleOrDefault(i => i.CourseID == course.CourseID);
                        context.Remove(courseToRemove);
                    }
                }
            }
        }
    }
}
