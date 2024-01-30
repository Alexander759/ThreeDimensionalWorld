
using Microsoft.AspNetCore.Identity;
using ThreeDimensionalWorld.DataAccess.Data;
using ThreeDimensionalWorld.Models;

namespace ThreeDimensionalWorldWeb.Configuration
{
    public class AppRolesAndUsersSeeder
    {

        private RoleManager<IdentityRole> _roleManager;
        private UserManager<ApplicationUser> _userManager;
        private ApplicationDbContext _context;


        public AppRolesAndUsersSeeder(RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }

        public async Task SeedDefaultRolesAndUsersIfEmpty()
        {
            await SeedDefaultRolesIfEmpty();
            await SeedDefaultUsersIfEmpty();
        }

        public async Task SeedDefaultRolesIfEmpty()
        {
            await SeedRoleIfEmpty(AppConfiguration.AdminRole);
            await SeedRoleIfEmpty(AppConfiguration.EmployeeRole);
            await SeedRoleIfEmpty(AppConfiguration.CustomerRole);
        }

        public async Task SeedRoleIfEmpty(string roleName)
        {
            if (!(await _roleManager.RoleExistsAsync(roleName)))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        public async Task SeedDefaultUsersIfEmpty()
        {
            await SeedDefaultUserIfEmpty(AppConfiguration.DefaultAdminUsername,
                AppConfiguration.DefaultAdminEmail,
                AppConfiguration.AdminRole,
                AppConfiguration.DefaultAdminPass);
        }

        public async Task SeedDefaultUserIfEmpty(string defaultUsername, 
            string defaultEmail, 
            string defaultRole, 
            string defaultPassword)
        {
            if (!(_userManager.Users.Any(user => user.Email == defaultEmail)))
            {

                ApplicationUser user = new ApplicationUser
                {
                    UserName = defaultUsername,
                    Email = defaultEmail
                };


                if(defaultRole == AppConfiguration.CustomerRole)
                {
                    Cart cart = new Cart() { UserId = user.Id };

                    _context.Add(cart);
                    await _context.SaveChangesAsync();

                    user.CartId = cart.Id;
                    user.Cart = cart;
                }

                await _userManager.CreateAsync(user, defaultPassword);

                await _userManager.AddToRoleAsync(user, defaultRole);

            }
        }
    }
}
