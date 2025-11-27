using Microsoft.AspNetCore.Identity;

namespace baraka.promo.Data
{
    public class AccountDatabaseInitializer
    {
        const string adminName = "admin";
        const string password = "admin";

        public static void Seed(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        public static void SeedUsers(UserManager<IdentityUser> userManager)
        {
            var admin = userManager.FindByNameAsync(adminName).Result;
            if (admin == null)
            {
                admin = new IdentityUser { UserName = adminName, Email = adminName, PhoneNumberConfirmed = true, EmailConfirmed = true };
                var result = userManager.CreateAsync(admin, password).Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(admin, adminName).Wait();
                }
            }
        }

        public static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            SeedRole(roleManager, adminName);
        }

        private static void SeedRole(RoleManager<IdentityRole> roleManager, string roleName)
        {
            var adminRole = roleManager.FindByNameAsync(roleName).Result;
            if (adminRole == null)
            {
                adminRole = new IdentityRole(roleName);
                roleManager.CreateAsync(adminRole).Wait();
            }
        }
    }
}
