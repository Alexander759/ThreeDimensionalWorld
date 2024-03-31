using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using ThreeDimensionalWorld.DataAccess.Repository.IRepository;
using ThreeDimensionalWorld.Models;
using ThreeDimensionalWorldWeb.Configuration;

namespace ThreeDimensionalWorldWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = AppConfiguration.AdminRole)]
    public class MaterialsController : Controller
    {
        private IUnitOfWork _unitOfWork;

        public MaterialsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var list =  _unitOfWork.MaterialRepository.GetAll("Colors");
            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Colors = _unitOfWork.MaterialColorRepository.GetAll();
            ViewBag.Selected = new List<int>();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Material material, List<int> colorIds)
        {
            if (ModelState.IsValid)
            {
                material.Colors = _unitOfWork.MaterialColorRepository.GetAll().Where(c => colorIds.Contains(c.Id)).ToList();
                _unitOfWork.MaterialRepository.Add(material);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }

            ViewBag.Colors = _unitOfWork.MaterialColorRepository.GetAll();
            ViewBag.Selected = colorIds;
            return View(material);
        }

        [HttpGet]
        public IActionResult Details(int? id)
        {
            if(id == null || id == 0)
            {
                return NotFound();
            }

            Material? material = _unitOfWork.MaterialRepository.Get(m => m.Id == id, "Colors");

            if(material == null)
            {
                return NotFound();
            }

            return View(material);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Material? material = _unitOfWork.MaterialRepository.Get(m => m.Id == id, "Colors");

            if (material == null)
            {
                return NotFound();
            }

            ViewBag.Colors = _unitOfWork.MaterialColorRepository.GetAll();
            ViewBag.Selected = material.Colors.Select(c => c.Id).ToList();
            return View(material);
        }

        [HttpPost]
        public IActionResult Edit(Material material, List<int> colorIds)
        {
            if (ModelState.IsValid)
            {
                material.Colors = _unitOfWork.MaterialColorRepository.GetAll().Where(c => colorIds.Contains(c.Id)).ToList();
                _unitOfWork.MaterialRepository.Update(material);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }

            ViewBag.Colors = _unitOfWork.MaterialColorRepository.GetAll();
            ViewBag.Selected = colorIds;
            return View(material);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Material? material = _unitOfWork.MaterialRepository.Get(m => m.Id == id, "Colors");

            if (material == null)
            {
                return NotFound();
            }

            ViewBag.Colors = _unitOfWork.MaterialColorRepository.GetAll();
            ViewBag.Selected = material.Colors.Select(c => c.Id).ToList();
            return View(material);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Material? materialColor = _unitOfWork.MaterialRepository.Get(c => c.Id == id, "Colors");

            if (materialColor == null)
            {
                return NotFound();
            }

            _unitOfWork.MaterialRepository.Remove(materialColor);
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }
    }
}