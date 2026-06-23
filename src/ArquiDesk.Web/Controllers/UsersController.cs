using ArquiDesk.Domain.Enums;
using ArquiDesk.Infrastructure.Identity;
using ArquiDesk.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArquiDesk.Web.Controllers;

[Authorize(Roles = UserRoles.Administrador)]
public class UsersController(UserManager<ApplicationUser> userManager) : Controller
{
    public async Task<IActionResult> Index()
    {
        var users = await userManager.Users
            .OrderBy(x => x.FullName)
            .ThenBy(x => x.Email)
            .ToListAsync();

        var model = new List<UserListItemViewModel>();
        foreach (var user in users)
        {
            model.Add(new UserListItemViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Department = user.Department,
                Active = user.Active,
                CreatedAt = user.CreatedAt,
                Roles = (await userManager.GetRolesAsync(user)).ToList()
            });
        }

        return View(model);
    }
}
