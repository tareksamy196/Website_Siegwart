using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Website.Siegwart.BLL.Dtos.Admin.SuperAdminAccount;
using Website.Siegwart.BLL.Services.Interfaces;

namespace Website.Siegwart.PL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    [Route("admin/[controller]")]
    public class SuperAdminController : BaseAdminController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAuditService _audit;
        private readonly ILogger<SuperAdminController> _logger;

        public SuperAdminController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IAuditService audit,
            ILogger<SuperAdminController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _audit = audit;
            _logger = logger;
        }

        // GET: admin/SuperAdmin or admin/SuperAdmin/Index
        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 30, CancellationToken cancellationToken = default)
        {
            try
            {
                if (page < 1) page = 1;
                pageSize = Math.Clamp(pageSize, 10, 200);
                var total = _userManager.Users.Count();
                var skip = (page - 1) * pageSize;

                var pageItems = _userManager.Users
                    .OrderBy(u => u.UserName)
                    .Skip(skip)
                    .Take(pageSize)
                    .Select(u => new
                    {
                        u.Id,
                        u.UserName,
                        u.Email,
                        u.EmailConfirmed
                    })
                    .ToList();

                var users = new List<UserListItemDto>(pageItems.Count);
                foreach (var u in pageItems)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var identityUser = new IdentityUser { Id = u.Id, UserName = u.UserName, Email = u.Email };
                    var roles = await _userManager.GetRolesAsync(identityUser);
                    users.Add(new UserListItemDto
                    {
                        Id = u.Id,
                        UserName = u.UserName ?? string.Empty,
                        Email = u.Email ?? string.Empty,
                        EmailConfirmed = u.EmailConfirmed,
                        Roles = roles
                    });
                }

                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.Total = total;
                ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);

                return View(users);
            }
            catch (Exception ex)
            {
                return HandleException(ex, _logger, nameof(Index), HttpContext.RequestServices.GetService(typeof(Microsoft.AspNetCore.Hosting.IWebHostEnvironment)) is Microsoft.AspNetCore.Hosting.IWebHostEnvironment env && env.IsDevelopment());
            }
        }

        // GET: admin/SuperAdmin/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            ViewBag.AllRoles = GetAllRoles();
            return View(new CreateAdminDto());
        }

        // POST: admin/SuperAdmin/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAdminDto model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.AllRoles = GetAllRoles();
                return View(model);
            }

            model.Normalize();

            if (!model.IsAgree)
            {
                ModelState.AddModelError(nameof(model.IsAgree), "You must agree to the terms.");
                ViewBag.AllRoles = GetAllRoles();
                return View(model);
            }

            const string adminRole = "Admin";
            try
            {
                if (!await _roleManager.RoleExistsAsync(adminRole))
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole(adminRole));
                    if (!roleResult.Succeeded)
                    {
                        _logger.LogError("Failed to create role {Role}: {Errors}", adminRole, string.Join("; ", roleResult.Errors.Select(e => e.Description)));
                        ModelState.AddModelError(string.Empty, "Unable to create required role.");
                        ViewBag.AllRoles = GetAllRoles();
                        return View(model);
                    }
                }

                if (await _userManager.FindByEmailAsync(model.Email) != null)
                {
                    ModelState.AddModelError(nameof(model.Email), "Email is already in use.");
                    ViewBag.AllRoles = GetAllRoles();
                    return View(model);
                }

                if (await _userManager.FindByNameAsync(model.UserName) != null)
                {
                    ModelState.AddModelError(nameof(model.UserName), "Username is already in use.");
                    ViewBag.AllRoles = GetAllRoles();
                    return View(model);
                }

                var user = new IdentityUser
                {
                    UserName = model.UserName,
                    Email = model.Email
                };

                var createResult = await _userManager.CreateAsync(user, model.Password);
                if (!createResult.Succeeded)
                {
                    foreach (var e in createResult.Errors)
                        ModelState.AddModelError(string.Empty, e.Description);

                    _logger.LogWarning("Failed to create admin {Email}: {Errors}", model.Email, string.Join("; ", createResult.Errors.Select(e => e.Description)));
                    ViewBag.AllRoles = GetAllRoles();
                    return View(model);
                }

                var addRoleResult = await _userManager.AddToRoleAsync(user, adminRole);
                if (!addRoleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    ModelState.AddModelError(string.Empty, "Failed to assign role to user.");
                    _logger.LogError("Failed to add created user {Email} to role {Role}: {Errors}", model.Email, adminRole, string.Join("; ", addRoleResult.Errors.Select(e => e.Description)));
                    ViewBag.AllRoles = GetAllRoles();
                    return View(model);
                }

                var creatorId = (await _userManager.GetUserAsync(User))?.Id ?? "system";
                await _audit.LogAsync(creatorId, "Admin.Created", user.Id, $"Admin account created by {creatorId}");

                TempData["Success"] = "Admin user created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return HandleException(ex, _logger, nameof(Create), HttpContext.RequestServices.GetService(typeof(Microsoft.AspNetCore.Hosting.IWebHostEnvironment)) is Microsoft.AspNetCore.Hosting.IWebHostEnvironment env && env.IsDevelopment());
            }
        }

        // GET: admin/SuperAdmin/Edit/{id}
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var userRoles = (await _userManager.GetRolesAsync(user)).ToList();
            var allRoles = GetAllRoles();

            var model = new EditAdminDto
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                EmailConfirmed = user.EmailConfirmed,
                Roles = userRoles
            };

            ViewBag.AllRoles = allRoles;
            return View(model);
        }

        // POST: admin/SuperAdmin/Edit/{id}
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditAdminDto model)
        {
            if (id != model.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewBag.AllRoles = GetAllRoles();
                return View(model);
            }

            model.UserName = model.UserName?.Trim() ?? string.Empty;
            model.Email = model.Email?.Trim().ToLowerInvariant() ?? string.Empty;
            model.Roles = model.Roles ?? new List<string>();

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            try
            {
                var currentRoles = (await _userManager.GetRolesAsync(user)).ToList();
                var rolesToRemove = currentRoles.Except(model.Roles, StringComparer.OrdinalIgnoreCase).ToList();
                var rolesToAdd = model.Roles.Except(currentRoles, StringComparer.OrdinalIgnoreCase).ToList();

                if (rolesToRemove.Any(r => string.Equals(r, "SuperAdmin", StringComparison.OrdinalIgnoreCase)))
                {
                    var superAdmins = await _userManager.GetUsersInRoleAsync("SuperAdmin");
                    if (superAdmins.Count == 1 && await _userManager.IsInRoleAsync(user, "SuperAdmin"))
                    {
                        ModelState.AddModelError(string.Empty, "Cannot remove SuperAdmin role — at least one SuperAdmin must exist.");
                        ViewBag.AllRoles = GetAllRoles();
                        return View(model);
                    }

                    var currentUserId = (await _userManager.GetUserAsync(User))?.Id;
                    if (string.Equals(currentUserId, user.Id, StringComparison.OrdinalIgnoreCase) &&
                        !model.Roles.Any(r => string.Equals(r, "SuperAdmin", StringComparison.OrdinalIgnoreCase)))
                    {
                        ModelState.AddModelError(string.Empty, "You cannot remove your own SuperAdmin role.");
                        ViewBag.AllRoles = GetAllRoles();
                        return View(model);
                    }
                }

                // Check duplicates for email/username
                var byEmail = await _userManager.FindByEmailAsync(model.Email);
                if (byEmail != null && byEmail.Id != user.Id)
                {
                    ModelState.AddModelError(nameof(model.Email), "Email is already used by another account.");
                    ViewBag.AllRoles = GetAllRoles();
                    return View(model);
                }

                var byUserName = await _userManager.FindByNameAsync(model.UserName);
                if (byUserName != null && byUserName.Id != user.Id)
                {
                    ModelState.AddModelError(nameof(model.UserName), "Username is already used by another account.");
                    ViewBag.AllRoles = GetAllRoles();
                    return View(model);
                }

                // Update user fields
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.EmailConfirmed = model.EmailConfirmed;

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    foreach (var e in updateResult.Errors)
                        ModelState.AddModelError(string.Empty, e.Description);

                    ViewBag.AllRoles = GetAllRoles();
                    return View(model);
                }

                // Add roles
                if (rolesToAdd.Any())
                {
                    var addRes = await _userManager.AddToRolesAsync(user, rolesToAdd);
                    if (!addRes.Succeeded)
                    {
                        foreach (var e in addRes.Errors)
                            ModelState.AddModelError(string.Empty, e.Description);

                        ViewBag.AllRoles = GetAllRoles();
                        return View(model);
                    }
                }

                // Remove roles
                if (rolesToRemove.Any())
                {
                    var remRes = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                    if (!remRes.Succeeded)
                    {
                        foreach (var e in remRes.Errors)
                            ModelState.AddModelError(string.Empty, e.Description);

                        ViewBag.AllRoles = GetAllRoles();
                        return View(model);
                    }
                }

                var editorId = (await _userManager.GetUserAsync(User))?.Id ?? "system";
                await _audit.LogAsync(editorId, "User.Updated", user.Id, $"User updated by {editorId}");

                TempData["Success"] = "User updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return HandleException(ex, _logger, nameof(Edit), HttpContext.RequestServices.GetService(typeof(Microsoft.AspNetCore.Hosting.IWebHostEnvironment)) is Microsoft.AspNetCore.Hosting.IWebHostEnvironment env && env.IsDevelopment());
            }
        }

        // GET: admin/SuperAdmin/Delete/{id}
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            var vm = new UserListItemDto
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                EmailConfirmed = user.EmailConfirmed,
                Roles = roles
            };

            return View(vm);
        }

        // POST: admin/SuperAdmin/Delete/{id}
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var currentUserId = (await _userManager.GetUserAsync(User))?.Id;
            if (string.Equals(currentUserId, user.Id, StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "You cannot delete your own account.";
                return RedirectToAction(nameof(Index));
            }

            if (await _userManager.IsInRoleAsync(user, "SuperAdmin"))
            {
                var superAdmins = await _userManager.GetUsersInRoleAsync("SuperAdmin");
                if (superAdmins.Count == 1)
                {
                    TempData["Error"] = "Cannot delete the last SuperAdmin account.";
                    return RedirectToAction(nameof(Index));
                }
            }

            try
            {
                var delResult = await _userManager.DeleteAsync(user);
                if (!delResult.Succeeded)
                {
                    TempData["Error"] = "Failed to delete user.";
                    _logger.LogError("Failed to delete user {Id}: {Errors}", user.Id, string.Join("; ", delResult.Errors.Select(e => e.Description)));
                    return RedirectToAction(nameof(Index));
                }

                var deleterId = (await _userManager.GetUserAsync(User))?.Id ?? "system";
                await _audit.LogAsync(deleterId, "User.Deleted", user.Id, $"User deleted by {deleterId}");

                TempData["Success"] = "User deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return HandleException(ex, _logger, nameof(DeleteConfirmed), HttpContext.RequestServices.GetService(typeof(Microsoft.AspNetCore.Hosting.IWebHostEnvironment)) is Microsoft.AspNetCore.Hosting.IWebHostEnvironment env && env.IsDevelopment());
            }
        }

        // GET: admin/SuperAdmin/ChangePassword/{id}
        [HttpGet("ChangePassword/{id}")]
        public async Task<IActionResult> ChangePassword(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var model = new AdminChangePasswordDto { UserId = user.Id };
            return View(model);
        }

        // POST: admin/SuperAdmin/ChangePassword/{id}
        [HttpPost("ChangePassword/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string id, AdminChangePasswordDto model)
        {
            if (id != model.UserId) return BadRequest();

            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();

            try
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                if (!resetResult.Succeeded)
                {
                    foreach (var e in resetResult.Errors)
                        ModelState.AddModelError(string.Empty, e.Description);

                    return View(model);
                }

                var editorId = (await _userManager.GetUserAsync(User))?.Id ?? "system";
                await _audit.LogAsync(editorId, "User.PasswordResetByAdmin", user.Id, $"Password reset by {editorId}");

                TempData["Success"] = "Password changed successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return HandleException(ex, _logger, nameof(ChangePassword), HttpContext.RequestServices.GetService(typeof(Microsoft.AspNetCore.Hosting.IWebHostEnvironment)) is Microsoft.AspNetCore.Hosting.IWebHostEnvironment env && env.IsDevelopment());
            }
        }
        private List<string> GetAllRoles()
        {
            return _roleManager.Roles.Select(r => r.Name!).ToList();
        }
    }
}