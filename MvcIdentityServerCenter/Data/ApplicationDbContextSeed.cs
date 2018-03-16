using Microsoft.AspNetCore.Identity;
using MvcIdentityServerCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace MvcIdentityServerCenter.Data
{
    public class ApplicationDbContextSeed
    {
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<ApplicationRole> _roleManager;

        public async Task SeedAsync(ApplicationDbContext context, IServiceProvider services)
        {
            if (!context.Roles.Any())
            {
                _roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

                var role = new ApplicationRole { Name = "Administrators", NormalizedName = "Administrators" };

                var result = await _roleManager.CreateAsync(role);

                if (!result.Succeeded)
                {
                    throw new Exception("初始化角色失败:" + result.Errors.SelectMany(e=> e.Description));
                }
            }

            if (!context.Users.Any())
            {
                _userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

                var defaultUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@126.com",
                    NormalizedUserName = "admin",
                    SecurityStamp = "admin",
                    Avatar = "https://chocolatey.org/content/packageimages/dotnetcore-runtime.install.2.0.5.png"
                };

                var result = await _userManager.CreateAsync(defaultUser, "123qwe");

                await _userManager.AddToRoleAsync(defaultUser, "Administrators");

                if (!result.Succeeded)
                {
                    throw new Exception("初始化认证用户失败");
                }
            }
        }
    }
}
