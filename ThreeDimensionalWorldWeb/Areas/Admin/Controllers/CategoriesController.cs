using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ThreeDimensionalWorld.DataAccess.Repository.IRepository;
using ThreeDimensionalWorld.Models;
using ThreeDimensionalWorldWeb.Areas.Admin.Models;
using ThreeDimensionalWorldWeb.Configuration;

namespace ThreeDimensionalWorldWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = AppConfiguration.AdminRole)]
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


            if (ModelState.IsValid)
            {
                Category category = new Category()
                {
                    Name = categoryVM.Name,
                    Image = await UploadFileAsync(categoryVM!.Image!, Path.Combine(_webHostEnvironment.WebRootPath, "images")),
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
        public async Task<IActionResult> Edit(CategoryVM categoryVM)
        {
            if (_unitOfWork.CategoryRepository.Get(c => c.Name == categoryVM.Name && c.Id != categoryVM.Id) != null)
            {
                ModelState.AddModelError("Name", "Категория с това име вече съществува");
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
                    await DeleteFileAsync(Path.Combine(_webHostEnvironment.WebRootPath, "images", category.Image));
                    category.Image = await UploadFileAsync(categoryVM.Image, Path.Combine(_webHostEnvironment.WebRootPath, "images"));
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

            await DeleteFileAsync(Path.Combine(_webHostEnvironment.WebRootPath, "images", category.Image));
            _unitOfWork.CategoryRepository.Remove(category);
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }

        public async Task<string> UploadFileAsync(IFormFile file, string pathToCopy)
        {
            // Generate a unique name using Guid
            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            // Combine the path to copy and the unique file name
            string fullPath = Path.Combine(pathToCopy, uniqueFileName);

            // Create the directory if it doesn't exist
            if (!Directory.Exists(pathToCopy))
            {
                Directory.CreateDirectory(pathToCopy);
            }

            // Copy the file to the specified path with the unique name
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return the new name of the file
            return uniqueFileName;
        }

        public async Task DeleteFileAsync(string filePath)
        {
            try
            {
                // Check if the file exists
                if (System.IO.File.Exists(filePath))
                {
                    // Delete the file asynchronously
                    await Task.Run(() => System.IO.File.Delete(filePath));
                    Console.WriteLine($"File at path '{filePath}' deleted successfully.");
                }
                else
                {
                    Console.WriteLine($"File at path '{filePath}' does not exist.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while deleting file at path '{filePath}': {ex.Message}");
            }
        }
    }
}
