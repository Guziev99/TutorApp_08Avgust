﻿using E_TutorApp.Application.Repositories.CategoryRepos;
using E_TutorApp.Domain.Entities.Concretes;
using E_TutorApp.Domain.ViewModels;
using E_TutorApp.Domain.ViewModels.CourseVMs;
using E_TutorApp.Persistence.Db_Contexts;
using E_TutorApp.Persistence.Repositories.CategoryRepos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_TutorApp.Web.Controllers
{
    public class StudentController : Controller
    {

        private readonly UserManager<User> _userManager;
        private TutorDbContext _context;
        private IWriteCategoryRepository _writeCategoryRepository;

        public StudentController(UserManager<User> userManager, TutorDbContext context, IWriteCategoryRepository writeCategoryRepository)
        {
            _userManager = userManager;
            _context = context;
            _writeCategoryRepository = writeCategoryRepository;
        }

        [HttpGet]
        public async Task< IActionResult> StudentDashboard()
        {

            //var categories = new List<Category>() {
            //    new Category () {Name = "Web Development", Description ="Programming"},
            //    new Category() {Name = "Design", Description = "About Design"}
            //};
            //await _context.Categories.AddRangeAsync(categories);
            //await _context.SaveChangesAsync();

            
            
            var user =  await _userManager.GetUserAsync(User);
            var userId = user!.Id;
            //var userDetail = _context.
            

            var enrolledCourses =  _context.Courses.Where(c => c.Students.Any(s => s.Id == userId)).ToList();

            
            //var c = await _context.Users.AnyAsync(s => s.Id == userId);
            //var enrolledCourses = _context.Courses.Find(c);

            //var completedCourses = enrolledCourses.Where(c => c.Status == CourseStatus.Completed).ToList();
            //var activeCourses = enrolledCourses.Where(c => c.Status == CourseStatus.Active).ToList();
            var completedCourses = enrolledCourses.Where(c => c.IsActive == false).ToList();
            var activeCourses = enrolledCourses.Where(c => c.IsActive == true).ToList();

           

            var model = new StudentDashboardVM
            {
                EnrolledCourseCount = enrolledCourses.Count,
                CompletedCourseCount = completedCourses.Count,
                ActiveCourseCount = activeCourses.Count,
                Courses = enrolledCourses
            };

            return View(model);
        }



        public async Task< IActionResult> StudentCourses(string? courseName = null, bool? latest = false, string? teacherName = null  )
        {
            //  Navbardaki Courses tiklandigda da bu calishsin. 
            // Hem de student sechim etdikde ( course ismini yazdigda, teachere gore axtardigda ve s )  yene bu action tetiklensin. 
            // Yuxaridaki latest parametresini buraya gondermeye ehtiyyac yoxdu. View terefdece yoxlanish edib hell edilmelidir dushunurem. 

            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;



            //var Courses = new List<Course>
            //{
            //    new Course { Name = "Course 1", Title = "Introduction to Course 1", ImageUrl = "course1.jpg" , Description = "About me", Price = 13, CategoryId = 1, InstructorId = 1 },
            //    new Course {  Name = "Course 2", Title = "Introduction to Course 2", ImageUrl = "course2.jpg",  Description = "About me", Price = 22, CategoryId = 1, InstructorId = 1 }
            //    // Add more courses as needed
            //};
            ////user!.DetailStudent!.EnrolledCoursesOfStudent!.ToList().AddRange(Courses);

            // _context.AddRange(Courses);
            //await _context.SaveChangesAsync();



          

            var enrolledCourses = _context.Courses.Where(c => c.Students!.Any(s => s.Id == userId)).ToList();


            var course =  _context.Courses.FirstOrDefault();
            enrolledCourses.Add(course);






            if (!string.IsNullOrEmpty(courseName))
            {
                var coursesOnCourseName = enrolledCourses.FindAll(c => c.Name.Contains(courseName));
                if (!string.IsNullOrEmpty(teacherName))
                {
                    var teacher = await _context.Users.FirstOrDefaultAsync(i => i.UserName == teacherName);
                    var CoursesOnTeacher = coursesOnCourseName.FindAll(c => c.InstructorId == teacher!.Id);
                    var viewModel1 = new CoursesViewModel
                    {
                        CourseName = courseName,
                        Filter = teacherName,
                        Courses = CoursesOnTeacher.ToList()
                    };
                    return View(viewModel1);
                }
                var viewModel2 = new CoursesViewModel
                {
                    CourseName = courseName,
                    Filter = teacherName,
                    Courses = coursesOnCourseName.ToList()
                };
                return View(viewModel2);
            }
            
            if (!string.IsNullOrEmpty(teacherName))
            {
                var teacher = await _context.Users.FirstOrDefaultAsync(i => i.UserName == teacherName);
                var CoursesOnTeacher = enrolledCourses.FindAll(c => c.InstructorId == teacher!.Id);
                var viewModel3 = new CoursesViewModel
                {
                    CourseName = courseName,
                    Filter = teacherName,
                    Courses = CoursesOnTeacher.ToList()
                };
                return View(viewModel3);
                
            }

            var viewModel = new CoursesViewModel
            {
                CourseName = courseName,
                Filter = teacherName,
                Courses = enrolledCourses.ToList()
            };
            return View(viewModel);




            //if (!string.IsNullOrEmpty(filter))
            //{
            //    switch (filter)
            //    {
            //        case "Latest":
            //            filteredCourses = filteredCourses.OrderByDescending(c => c.Id); // Assuming Id represents the creation order
            //            break;
            //        case "AllCourses":
            //            // No additional filtering needed for AllCourses
            //            break;
            //        case "AllTeachers":
            //            // Implement logic to filter by teachers if needed
            //            break;
            //    }



                //if ( courseName == null & teacherName != null ) 
                //{
                //    var teacherId =  _context.Users.Where(t=>t.UserName == teacherName).FirstOrDefault()!.Id;
                //    var courses =   enrolledCourses.Where(c => c.InstructorId == teacherId);

                //    return View(courses);
                //}

                //return View(enrolledCourses);
            }



    }
}
