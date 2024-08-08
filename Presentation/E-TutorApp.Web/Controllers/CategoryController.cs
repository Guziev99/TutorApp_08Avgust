using E_TutorApp.Application.Repositories.CategoryRepos;
using E_TutorApp.Domain.Entities.Concretes;
using E_TutorApp.Domain.ViewModels;
using EStore_Domain.ViewModels;
using EStore_Persistence.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_TutorApp.Web.Controllers
{
    public class CategoryController : Controller
    {
        private  IWriteCategoryRepository _writeCategoryRepository;
        private readonly IReadCategoryRepository _readCategoryRepository;

        public CategoryController(IWriteCategoryRepository writeCategoryRepository, IReadCategoryRepository readCategoryRepository)
        { 
            _writeCategoryRepository = writeCategoryRepository;
                _readCategoryRepository = readCategoryRepository;
        
        }


        #region AddCategory
        [HttpGet]
        public ActionResult AddCategory() 
        {
            return View();
        }

        [HttpPost]
        public async Task< IActionResult> AddCategory(GetCategoryVM categoryVM)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var newcategory = new Category()
            {
                Name = categoryVM.Name,
                Description = categoryVM.Description,
            };
            await _writeCategoryRepository.AddAsync(newcategory);
            await _writeCategoryRepository.SaveChangeAsync();
        
            return View();
        }
        #endregion


        #region GetAllCategories
        [HttpGet]
        public async Task <IActionResult> GetAllCategories()
        {
            if (!ModelState.IsValid) return View("Error");

            var categories = await _readCategoryRepository.GetAllAsync();
            var categoriesOnPages = PaginationService.Paginate<Category>(categories, new PaginationVM() { Page = 2, PageSize = 3 });
            var categoryVmPages =  categoriesOnPages!.Select(c =>
                new CategoryVM()
                {
                    Name = c.Name,
                    Description = c.Description,
                }  );
            return View(categoryVmPages);
        }

        #endregion


        #region GetCoursesByCategoryByName

        [HttpGet]
        public  async Task<  IActionResult> GetCoursesByCategoryName(string name)
        {

            var courses = await _readCategoryRepository.GetAllCoursesWithCategoryName(name);
               ViewBag .Courses = courses;
               return View(courses);
        }

        //[HttpPost]
        //public async Task<IActionResult> GetCoursesByCategoryId(int id)
        //{
        //    var courses = await _readCategoryRepository.GetAllCoursesWithCategoryId(id);

        //    ViewBag .Courses = courses;
        //    return View("AllCoursesThisCategory", courses);
        //}

        //[HttpGet]
        //public async Task <IActionResult> AllCoursesThisCategory(IEnumerable<Course>? courses )
        //{
        //    return View(courses);
        //}



        #endregion




    }
}
