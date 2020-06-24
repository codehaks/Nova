using Microsoft.AspNetCore.Identity;
using Portal.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Web.Common
{
    public class SeedIdentityData
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedIdentityData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task Init()
        {
            await SeedRoles();
            await SeedUsers();
        }

        private async Task SeedRoles()
        {
            await _roleManager.CreateAsync(new IdentityRole("admin"));
            
        }

        private async Task SeedUsers()
        {
            var user = await _userManager
                .FindByNameAsync("admin@damoon.com");
            if (user == null)
            {
                var adminUser = new ApplicationUser
                {
                    Email = "admin@damoon.com",
                    UserName = "admin@damoon.com",
                };
                await _userManager.CreateAsync(adminUser,"123@Abc");

                await _userManager.AddToRoleAsync(adminUser, "admin");
            }
        }
    }
}
