using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Serwis_Książkowy.Models;

namespace Serwis_Książkowy.Helpers;

public static class UserHelper
{
    public static string GetUserId(this ClaimsPrincipal user)
    {
        var userId=  user.FindFirstValue(ClaimTypes.NameIdentifier);
        userId = userId ?? string.Empty;
        return userId;
    }

    public static async Task CreateRoles(IServiceScope scope)
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        var roles = new[] { "Admin", "User" };

        foreach (var roleName in roles)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }

    public static async Task UserManager(IServiceScope scope)
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        string email = "admin@admin.com";
        string password = "123123";
        if (await userManager.FindByEmailAsync(email) == null)
        {
            var user = new AppUser()
            {
                UserName = email,
                Email = email
            };
            await userManager.CreateAsync(user, password);
            await userManager.AddToRoleAsync(user, "admin");
        }
    }
}