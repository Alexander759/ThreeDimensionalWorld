using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ThreeDimensionalWorld.DataAccess.Repository.IRepository;
using ThreeDimensionalWorld.Models;
using ThreeDimensionalWorldWeb.Configuration;

namespace ThreeDimensionalWorldWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = AppConfiguration.AdminRole)]
    public class ApplicationUsersController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ApplicationUsersController(IUnitOfWork unitOfWork, 
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View(_unitOfWork.ApplicationUserRepository.GetAll());
        }

        public IActionResult Details(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            ApplicationUser? applicationUser = _unitOfWork.ApplicationUserRepository.Get(u => u.Id == id);

            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
        }

        public IActionResult Delete(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            ApplicationUser? applicationUser = _unitOfWork.ApplicationUserRepository.Get(u => u.Id == id);

            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            ApplicationUser? applicationUser = _unitOfWork.ApplicationUserRepository.Get(u => u.Id == id);

            if (applicationUser == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(applicationUser);
            
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }
        }
    }
}

