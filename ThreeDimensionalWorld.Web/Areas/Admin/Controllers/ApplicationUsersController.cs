using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ThreeDimensionalWorld.DataAccess.Repository.IRepository;
using ThreeDimensionalWorld.Models;
using ThreeDimensionalWorld.Web.RolesAndUsersConfiguration;
using ThreeDimensionalWorld.Web.Models;

namespace ThreeDimensionalWorld.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = AppRolesAndUsersConfiguration.AdminRole)]
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

        [HttpGet]
        public IActionResult Index()
        {
            return View(_unitOfWork.ApplicationUserRepository.GetAll());
        }

        [HttpGet]
        public IActionResult ManageRoles(string? id)
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

            List<IdentityRole> roles = _roleManager.Roles.ToList();

            Dictionary<IdentityRole, string> dictionary = new Dictionary<IdentityRole, string>();

            foreach (var role in roles)
            {
                string translate = "";
                if(role.Name == AppRolesAndUsersConfiguration.AdminRole)
                {
                    translate = "Администратор";
                }

                if (role.Name == AppRolesAndUsersConfiguration.CustomerRole)
                {
                    translate = "Клиент";
                }

                dictionary.Add(role, translate);
            }

            ViewData["RolesDictionary"] = dictionary;
            ViewData["UserManager"] = _userManager;

            return View(applicationUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRoles(string? id, List<string> roleNames)
        {
            if(string.IsNullOrEmpty(id) || roleNames == null)
            {
                return BadRequest();
            }

            ApplicationUser? applicationUser = _unitOfWork.ApplicationUserRepository.Get(u => u.Id == id);

            if (applicationUser == null)
            {
                return NotFound();
            }

            if (applicationUser.UserName == AppRolesAndUsersConfiguration.DefaultAdminUsername)
            {
                return StatusCode(405, "You can't change roles of first admin");
            }

            List<IdentityRole> roles = _roleManager.Roles.ToList();

            foreach (IdentityRole role in roles)
            {
                if(await _userManager.IsInRoleAsync(applicationUser, role.Name!))
                {
                    await _userManager.RemoveFromRoleAsync(applicationUser, role.Name!);
                }
            }

            foreach (string role in roleNames)
            {
                if(await _roleManager.RoleExistsAsync(role))
                {
                     await _userManager.AddToRoleAsync(applicationUser, role);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
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

            List<IdentityRole> roles = _roleManager.Roles.ToList();

            Dictionary<IdentityRole, string> dictionary = new Dictionary<IdentityRole, string>();

            foreach (var role in roles)
            {
                string translate = "";
                if (role.Name == AppRolesAndUsersConfiguration.AdminRole)
                {
                    translate = "Администратор";
                }

                if (role.Name == AppRolesAndUsersConfiguration.CustomerRole)
                {
                    translate = "Клиент";
                }

                dictionary.Add(role, translate);
            }

            ViewData["RolesDictionary"] = dictionary;
            ViewData["UserManager"] = _userManager;

            return View(applicationUser);
        }

        [HttpGet]
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

            List<IdentityRole> roles = _roleManager.Roles.ToList();

            Dictionary<IdentityRole, string> dictionary = new Dictionary<IdentityRole, string>();

            foreach (var role in roles)
            {
                string translate = "";
                if (role.Name == AppRolesAndUsersConfiguration.AdminRole)
                {
                    translate = "Администратор";
                }

                if (role.Name == AppRolesAndUsersConfiguration.CustomerRole)
                {
                    translate = "Клиент";
                }

                dictionary.Add(role, translate);
            }

            ViewData["RolesDictionary"] = dictionary;
            ViewData["UserManager"] = _userManager;

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

            if(applicationUser.UserName == AppRolesAndUsersConfiguration.DefaultAdminUsername)
            {
                return StatusCode(405, "You can't delete first admin");
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

