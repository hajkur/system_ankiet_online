using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using zad1.Models.ViewModels;

namespace zad1.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<AdminController> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var users = _userManager.Users.ToList();
        var viewModel = new UserManagementViewModel
        {
            AvailableRoles = _roleManager.Roles.Select(r => r.Name!).ToList()
        };

        foreach (var user in users)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            viewModel.Users.Add(new UserWithRolesViewModel
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                Roles = userRoles.ToList()
            });
        }

        return View(viewModel);
    }

    public async Task<IActionResult> ManageRoles(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        var allRoles = _roleManager.Roles.ToList();

        var viewModel = new ManageUserRolesViewModel
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            Roles = allRoles.Select(role => new RoleCheckboxViewModel
            {
                RoleName = role.Name!,
                IsSelected = userRoles.Contains(role.Name!)
            }).ToList()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ManageRoles(ManageUserRolesViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null)
        {
            return NotFound();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        var isManagingSelf = currentUser?.Id == user.Id;

        var userRoles = await _userManager.GetRolesAsync(user);
        var selectedRoles = model.Roles.Where(r => r.IsSelected).Select(r => r.RoleName).ToList();

        if (isManagingSelf && userRoles.Contains("Admin") && !selectedRoles.Contains("Admin"))
        {
            TempData["ErrorMessage"] = "You cannot remove the Admin role from yourself.";
            return RedirectToAction(nameof(ManageRoles), new { userId = model.UserId });
        }

        var rolesToRemove = userRoles.Except(selectedRoles).ToList();
        var rolesToAdd = selectedRoles.Except(userRoles).ToList();

        if (rolesToRemove.Any())
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            if (!removeResult.Succeeded)
            {
                foreach (var error in removeResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }
        }

        if (rolesToAdd.Any())
        {
            var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
            if (!addResult.Succeeded)
            {
                foreach (var error in addResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }
        }

        TempData["SuccessMessage"] = $"Roles updated successfully for {user.Email}.";
        return RedirectToAction(nameof(Index));
    }
}
