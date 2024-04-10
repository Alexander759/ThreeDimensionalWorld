using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThreeDimensionalWorld.DataAccess.Repository.IRepository;
using ThreeDimensionalWorld.Models;
using ThreeDimensionalWorld.Web.RolesAndUsersConfiguration;

namespace ThreeDimensionalWorld.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = AppRolesAndUsersConfiguration.AdminRole)]
    public class MaterialColorsController : Controller
    {
        private IUnitOfWork _unitOfWork;

        public MaterialColorsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(_unitOfWork.MaterialColorRepository.GetAll());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MaterialColor materialColor)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.MaterialColorRepository.Add(materialColor);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View(materialColor);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            MaterialColor? materialColor = _unitOfWork.MaterialColorRepository.Get(c => c.Id == id);
            
            if(materialColor == null)
            {
                return NotFound();
            }

            return View(materialColor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(MaterialColor materialColor)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.MaterialColorRepository.Update(materialColor);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View(materialColor);
        }

        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MaterialColor? materialColor = _unitOfWork.MaterialColorRepository.Get(c => c.Id == id);

            if (materialColor == null)
            {
                return NotFound();
            }

            return View(materialColor);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MaterialColor? materialColor = _unitOfWork.MaterialColorRepository.Get(c => c.Id == id);

            if (materialColor == null)
            {
                return NotFound();
            }

            return View(materialColor);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MaterialColor? materialColor = _unitOfWork.MaterialColorRepository.Get(c => c.Id == id);

            if (materialColor == null)
            {
                return NotFound();
            }

            _unitOfWork.MaterialColorRepository.Remove(materialColor);
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }
    }
}
