using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.AccessControl;
using ThreeDimensionalWorld.DataAccess.Repository.IRepository;
using ThreeDimensionalWorld.Models;
using ThreeDimensionalWorld.Utility;

namespace ThreeDimensionalWorld.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _webHostEnvironment;

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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, List<IFormFile> files)
        {
            if(files == null || files.Count < 2)
            {
                ModelState.AddModelError(string.Empty, "Нужно е да добавите поне 2 файла");
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.ProductRepository.Add(product);
                _unitOfWork.Save();
                for (int i = 0; i < files!.Count; i++)
                {
                    IFormFile file = files[i];
                    string uniqueFileName = null!;
                    if (AllowedFormats.Allowed3dFormats.Contains(Path.GetExtension(file.FileName)))
                    {
                        uniqueFileName = await FileManager.UploadFileAsync(file, Path.Combine(_webHostEnvironment.WebRootPath, "3dModels"));
                    }
                    else if (AllowedFormats.AllowedImageFormats.Contains(Path.GetExtension(file.FileName)))
                    {
                        uniqueFileName = await FileManager.UploadFileAsync(file, Path.Combine(_webHostEnvironment.WebRootPath, "images"));
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
                        if (AllowedFormats.Allowed3dFormats.Contains(Path.GetExtension(item.Name)))
                        {
                            await FileManager.DeleteFileAsync(Path.Combine(_webHostEnvironment.WebRootPath, "3dModels", item.Name));
                        }
                        else if (AllowedFormats.AllowedImageFormats.Contains(Path.GetExtension(item.Name)))
                        {
                            await FileManager.DeleteFileAsync(Path.Combine(_webHostEnvironment.WebRootPath, "images", item.Name));
                        }

                        _unitOfWork.ProductFileRepository.Remove(item);
                        _unitOfWork.Save();
                    }


                    for (int i = 0; i < files.Count; i++)
                    {
                        IFormFile file = files[i];
                        string uniqueFileName = null!;
                        if (AllowedFormats.Allowed3dFormats.Contains(Path.GetExtension(file.FileName)))
                        {
                            uniqueFileName = await FileManager.UploadFileAsync(file, Path.Combine(_webHostEnvironment.WebRootPath, "3dModels"));
                        }
                        else if (AllowedFormats.AllowedImageFormats.Contains(Path.GetExtension(file.FileName)))
                        {
                            uniqueFileName = await FileManager.UploadFileAsync(file, Path.Combine(_webHostEnvironment.WebRootPath, "images"));
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
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
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
                if (AllowedFormats.Allowed3dFormats.Contains(Path.GetExtension(item.Name)))
                {
                    await FileManager.DeleteFileAsync(Path.Combine(_webHostEnvironment.WebRootPath, "3dModels", item.Name));
                }
                else if (AllowedFormats.AllowedImageFormats.Contains(Path.GetExtension(item.Name)))
                {
                    await FileManager.DeleteFileAsync(Path.Combine(_webHostEnvironment.WebRootPath, "images", item.Name));
                }

                _unitOfWork.ProductFileRepository.Remove(item);
                _unitOfWork.Save();
            }

            _unitOfWork.ProductRepository.Remove(product);
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }
    }
}