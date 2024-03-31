using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.AccessControl;
using ThreeDimensionalWorld.DataAccess.Repository.IRepository;
using ThreeDimensionalWorld.Models;

namespace ThreeDimensionalWorldWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _webHostEnvironment;

        private static string[] allowedImageFormats =
        [
            ".png",
            ".jpg",
            ".gif"
        ];

        private static string[] allowed3dFormats =
        {
            ".stl",
        };

        public ProductsController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<Product> products = _unitOfWork.ProductRepository.GetAll("Category").ToList();
            return View(products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Categories"] = new SelectList(_unitOfWork.CategoryRepository.GetAll(), "Id", "Name");
            ViewData["Allowed3dFormats"] = allowed3dFormats.ToList();
            ViewData["AllowedImagesFormats"] = allowedImageFormats.ToList();
            ViewData["AllowedFormats"] = (List<string>)[.. allowedImageFormats, .. allowed3dFormats];

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.ProductRepository.Add(product);
                _unitOfWork.Save();
                for (int i = 0; i < files.Count; i++)
                {
                    IFormFile file = files[i];
                    string uniqueFileName = null!;
                    if (allowed3dFormats.Contains(Path.GetExtension(file.FileName)))
                    {
                        uniqueFileName = await UploadFileAsync(file, Path.Combine(_webHostEnvironment.WebRootPath, "3dModels"));
                    }
                    else if (allowedImageFormats.Contains(Path.GetExtension(file.FileName)))
                    {
                        uniqueFileName = await UploadFileAsync(file, Path.Combine(_webHostEnvironment.WebRootPath, "images"));
                    }
                    else
                    {
                        continue;
                    }

                    ProductFile productFile = new ProductFile() { Name = uniqueFileName, Order = i, ProductId = product.Id };
                    _unitOfWork.ProductFileRepository.Add(productFile);
                    _unitOfWork.Save();
                }

                return RedirectToAction("Index");
            }

            ViewData["Categories"] = new SelectList(_unitOfWork.CategoryRepository.GetAll(), "Id", "Name");
            ViewData["Allowed3dFormats"] = allowed3dFormats.ToList();
            ViewData["AllowedImagesFormats"] = allowedImageFormats.ToList();
            ViewData["AllowedFormats"] = (List<string>)[.. allowedImageFormats, .. allowed3dFormats];
            return View(product);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if(id == null || id == 0)
            {
                return NotFound();
            }

            Product? product = _unitOfWork.ProductRepository.Get(p => p.Id == id, "Files,Category");
            
            if(product == null)
            {
                return NotFound();
            }

            ViewData["Categories"] = new SelectList(_unitOfWork.CategoryRepository.GetAll(), "Id", "Name");
            ViewData["Allowed3dFormats"] = allowed3dFormats.ToList();
            ViewData["AllowedImagesFormats"] = allowedImageFormats.ToList();
            ViewData["AllowedFormats"] = (List<string>)[.. allowedImageFormats, .. allowed3dFormats];

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product product, List<IFormFile>? files)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.ProductRepository.Update(product);
                _unitOfWork.Save();

                if(files != null && files.Count > 0)
                {
                    Product? productWithFiles = _unitOfWork.ProductRepository.Get(p => p.Id == product.Id, "Files,Category");

                    if(productWithFiles == null)
                    {
                        return NotFound();
                    }

                    List<ProductFile> oldFilesList = productWithFiles.Files.ToList();

                    for (int i = 0; i <oldFilesList.Count; i++)
                    {
                        ProductFile item = oldFilesList[i];
                        if (allowed3dFormats.Contains(Path.GetExtension(item.Name)))
                        {
                            await DeleteFileAsync(Path.Combine(_webHostEnvironment.WebRootPath, "3dModels", item.Name));
                        }
                        else if (allowedImageFormats.Contains(Path.GetExtension(item.Name)))
                        {
                            await DeleteFileAsync(Path.Combine(_webHostEnvironment.WebRootPath, "images", item.Name));
                        }

                        _unitOfWork.ProductFileRepository.Remove(item);
                        _unitOfWork.Save();
                    }


                    for (int i = 0; i < files.Count; i++)
                    {
                        IFormFile file = files[i];
                        string uniqueFileName = null!;
                        if (allowed3dFormats.Contains(Path.GetExtension(file.FileName)))
                        {
                            uniqueFileName = await UploadFileAsync(file, Path.Combine(_webHostEnvironment.WebRootPath, "3dModels"));
                        }
                        else if (allowedImageFormats.Contains(Path.GetExtension(file.FileName)))
                        {
                            uniqueFileName = await UploadFileAsync(file, Path.Combine(_webHostEnvironment.WebRootPath, "images"));
                        }
                        else
                        {
                            continue;
                        }

                        ProductFile productFile = new ProductFile() { Name = uniqueFileName, Order = i, ProductId = product.Id };
                        _unitOfWork.ProductFileRepository.Add(productFile);
                        _unitOfWork.Save();
                       }

                }

                return RedirectToAction("Index");
            }

            ViewData["Categories"] = new SelectList(_unitOfWork.CategoryRepository.GetAll(), "Id", "Name");
            ViewData["Allowed3dFormats"] = allowed3dFormats.ToList();
            ViewData["AllowedImagesFormats"] = allowedImageFormats.ToList();
            ViewData["AllowedFormats"] = (List<string>)[.. allowedImageFormats, .. allowed3dFormats];

            return View(product);

        }

        public IActionResult Details(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Product? product = _unitOfWork.ProductRepository.Get(p => p.Id == id, "Files,Category");

            if (product == null)
            {
                return NotFound();
            }

            ViewData["Categories"] = new SelectList(_unitOfWork.CategoryRepository.GetAll(), "Id", "Name");
            ViewData["Allowed3dFormats"] = allowed3dFormats.ToList();
            ViewData["AllowedImagesFormats"] = allowedImageFormats.ToList();
            ViewData["AllowedFormats"] = (List<string>)[.. allowedImageFormats, .. allowed3dFormats];

            return View(product);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Product? product = _unitOfWork.ProductRepository.Get(p => p.Id == id, "Files,Category");

            if (product == null)
            {
                return NotFound();
            }

            ViewData["Categories"] = new SelectList(_unitOfWork.CategoryRepository.GetAll(), "Id", "Name");
            ViewData["Allowed3dFormats"] = allowed3dFormats.ToList();
            ViewData["AllowedImagesFormats"] = allowedImageFormats.ToList();
            ViewData["AllowedFormats"] = (List<string>)[.. allowedImageFormats, .. allowed3dFormats];

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Product? product = _unitOfWork.ProductRepository.Get(p => p.Id == id, "Files,Category");

            if (product == null)
            {
                return NotFound();
            }

            List<ProductFile> oldFilesList = product.Files.ToList();

            for (int i = 0; i < oldFilesList.Count; i++)
            {
                ProductFile item = oldFilesList[i];
                if (allowed3dFormats.Contains(Path.GetExtension(item.Name)))
                {
                    await DeleteFileAsync(Path.Combine(_webHostEnvironment.WebRootPath, "3dModels", item.Name));
                }
                else if (allowedImageFormats.Contains(Path.GetExtension(item.Name)))
                {
                    await DeleteFileAsync(Path.Combine(_webHostEnvironment.WebRootPath, "images", item.Name));
                }

                _unitOfWork.ProductFileRepository.Remove(item);
                _unitOfWork.Save();
            }

            _unitOfWork.ProductRepository.Remove(product);
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
            if(!Directory.Exists(pathToCopy))
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