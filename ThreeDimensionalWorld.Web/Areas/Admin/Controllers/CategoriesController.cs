using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ThreeDimensionalWorld.DataAccess.Repository.IRepository;
using ThreeDimensionalWorld.Models;
using ThreeDimensionalWorld.Utility;
using ThreeDimensionalWorld.Web.Areas.Admin.Models;
using ThreeDimensionalWorld.Web.RolesAndUsersConfiguration;

namespace ThreeDimensionalWorld.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = AppRolesAndUsersConfiguration.AdminRole)]
    public class CategoriesController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _webHostEnvironment;

        public CategoriesController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<Category> categories = _unitOfWork.CategoryRepository.GetAll().ToList();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryVM categoryVM)
        {
            if (_unitOfWork.CategoryRepository.Get(c => c.Name == categoryVM.Name) != null)
            {
                ModelState.AddModelError("Name", "Категория с това име вече съществува");
            }

            if(categoryVM.Image == null)
            {
                ModelState.AddModelError("Image", "Няма прикачена снимка");
            }
            else
            {
                if (!AllowedFormats.AllowedImageFormats.Contains(Path.GetExtension(categoryVM.Image.FileName)))
                {
                    ModelState.AddModelError("Image", "Снимката е в неподдържан формат");
                }
            }

            if (ModelState.IsValid)
            {
                Category category = new Category()
                {
                    Name = categoryVM.Name,
                    Image = await FileManager.UploadFileAsync(categoryVM!.Image!, Path.Combine(_webHostEnvironment.WebRootPath, "images")),
                    Description = categoryVM!.Description
                };

                _unitOfWork.CategoryRepository.Add(category);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View(categoryVM);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Category? category = _unitOfWork.CategoryRepository.Get(c => c.Id == id);
            
            if(category == null)
            {
                return NotFound();
            }

            CategoryVM categoryVM = new CategoryVM()
            {
                Id=category.Id,
                Name = category.Name,
                Description = category.Description
            };

            return View(categoryVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryVM categoryVM)
        {
            if (_unitOfWork.CategoryRepository.Get(c => c.Name == categoryVM.Name && c.Id != categoryVM.Id) != null)
            {
                ModelState.AddModelError("Name", "Категория с това име вече съществува");
            }

            if (categoryVM.Image != null && !AllowedFormats.AllowedImageFormats.Contains(Path.GetExtension(categoryVM.Image.FileName)))
            {
                ModelState.AddModelError("Image", "Снимката е в неподдържан формат");
            }

            if (ModelState.IsValid)
            {
                Category? category = _unitOfWork.CategoryRepository.Get(c => c.Id == categoryVM.Id);

                if(category == null)
                {
                    return NotFound();
                }

                category.Name = categoryVM.Name;
                category.Description = categoryVM.Description;
                
                if(categoryVM.Image!= null)
                {
                    await FileManager.DeleteFileAsync(Path.Combine(_webHostEnvironment.WebRootPath, "images", category.Image));
                    category.Image = await FileManager.UploadFileAsync(categoryVM.Image, Path.Combine(_webHostEnvironment.WebRootPath, "images"));
                }

                _unitOfWork.CategoryRepository.Update(category);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View(categoryVM);
        }

        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Category? category = _unitOfWork.CategoryRepository.Get(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Category? category = _unitOfWork.CategoryRepository.Get(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePOST(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Category? category = _unitOfWork.CategoryRepository.Get(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            await FileManager.DeleteFileAsync(Path.Combine(_webHostEnvironment.WebRootPath, "images", category.Image));
            _unitOfWork.CategoryRepository.Remove(category);
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }
    }
}
