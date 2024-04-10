
using Microsoft.AspNetCore.Identity;
using ThreeDimensionalWorld.DataAccess.Data;
using ThreeDimensionalWorld.Models;

namespace ThreeDimensionalWorld.Web.RolesAndUsersConfiguration
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

        public async Task SeedDefaultRolesAndUsersIfEmptyAsync()
        {
            await SeedDefaultRolesIfEmptyAsync();
            await SeedDefaultUsersIfEmptyAsync();
        }

        public async Task SeedDefaultRolesIfEmptyAsync()
        {
            await SeedRoleIfEmptyAsync(AppRolesAndUsersConfiguration.AdminRole);
            await SeedRoleIfEmptyAsync(AppRolesAndUsersConfiguration.CustomerRole);
        }

        public async Task SeedRoleIfEmptyAsync(string roleName)
        {
            if (!(await _roleManager.RoleExistsAsync(roleName)))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        public async Task SeedDefaultUsersIfEmptyAsync()
        {
            await SeedDefaultUserIfEmptyAsync(AppRolesAndUsersConfiguration.DefaultAdminUsername,
                AppRolesAndUsersConfiguration.DefaultAdminEmail,
                AppRolesAndUsersConfiguration.AdminRole,
                AppRolesAndUsersConfiguration.DefaultAdminFirstName,
                AppRolesAndUsersConfiguration.DefaultAdminLastName,
                AppRolesAndUsersConfiguration.DefaultAdminPhoneNumber,
                AppRolesAndUsersConfiguration.DefaultAdminPass);

            await SeedDefaultUserIfEmptyAsync(AppRolesAndUsersConfiguration.DefaultUserUsername,
                AppRolesAndUsersConfiguration.DefaultUserEmail,
                AppRolesAndUsersConfiguration.CustomerRole,
                AppRolesAndUsersConfiguration.DefaultUserFirstName,
                AppRolesAndUsersConfiguration.DefaultUserLastName,
                AppRolesAndUsersConfiguration.DefaultUserPhoneNumber,
                AppRolesAndUsersConfiguration.DefaultUserPass);

        }

        public async Task SeedDefaultUserIfEmptyAsync (string defaultUsername, 
            string defaultEmail, 
            string defaultRole,
            string defaultFirstName,
            string defaultLastName,
            string defaultPhoneNumber,
            string defaultPassword)
        {
            if (!(_userManager.Users.Any(user => user.Email == defaultEmail)))
            {

                ApplicationUser user = new ApplicationUser
                {
                    UserName = defaultUsername,
                    Email = defaultEmail,
                    FirstName = defaultFirstName,
                    LastName = defaultLastName,
                    PhoneNumber = defaultPhoneNumber,
                    EmailConfirmed = true
                };

                await _userManager.CreateAsync(user, defaultPassword);

                await _userManager.AddToRoleAsync(user, defaultRole);

                if (defaultRole == AppRolesAndUsersConfiguration.CustomerRole)
                {
                    ShoppingCart cart = new ShoppingCart() { UserId = user.Id };

                    await _context.ShoppingCarts.AddAsync(cart);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
