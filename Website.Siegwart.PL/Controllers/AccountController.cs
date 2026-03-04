using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Website.Siegwart.BLL.Dtos.Account;
using Website.Siegwart.BLL.Dtos.Admin.AccountDtos;
using Website.Siegwart.BLL.Services.Interfaces;
using Email = Website.Siegwart.DAL.Models.Email;

namespace Website.Siegwart.PL.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IAppEmailSender _emailSender;
        private readonly IAuditService _audit;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IAppEmailSender emailSender,
            IAuditService audit,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _audit = audit;
            _logger = logger;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                _logger.LogDebug("Authenticated user {User}. Redirecting to admin area.", User.Identity?.Name);
                return RedirectToAction("Index", "AdminDashboard");
            }

            var model = new LoginAdminDto { ReturnUrl = returnUrl };
            return View(model);
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginAdminDto? model)
        {
            if (model == null)
            {
                _logger.LogWarning("Login model is null.");
                ModelState.AddModelError(string.Empty, "Invalid request.");
                return View(new LoginAdminDto());
            }

            model.Normalize();

            if (!ModelState.IsValid)
            {
                _logger.LogDebug("Login model state invalid for {Email}.", model.Email);
                return View(model);
            }

            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    _logger.LogInformation("Login failed (not found) for {Email}", model.Email);
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                    await _audit.LogAsync("unknown", "Login.Failed", "unknown", $"Login failed for email {model.Email}");
                    return View(model);
                }

                var signInResult = await _signInManager.PasswordSignInAsync(
                    userName: user.UserName ?? string.Empty,
                    password: model.Password,
                    isPersistent: model.RememberMe,
                    lockoutOnFailure: true);

                if (signInResult.Succeeded)
                {
                    _logger.LogInformation("User {Email} logged in", model.Email);
                    await _audit.LogAsync(user.Id, "Login.Success", user.Id, "User logged in");

                    if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                        return LocalRedirect(model.ReturnUrl);

                    return RedirectToAction("Index", "AdminDashboard");
                }

                if (signInResult.IsLockedOut)
                {
                    _logger.LogWarning("User {Email} locked out", model.Email);

                    var lockoutUser = await _userManager.FindByIdAsync(user.Id);
                    if (lockoutUser?.LockoutEnd.HasValue == true)
                    {
                        var minutesLeft = Math.Max(0, Math.Ceiling((lockoutUser.LockoutEnd.Value.UtcDateTime - DateTime.UtcNow).TotalMinutes));
                        ModelState.AddModelError(string.Empty, $"Account locked. Try again in {minutesLeft} minute(s).");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Account locked. Contact administrator.");
                    }

                    await _audit.LogAsync(user.Id, "Login.LockedOut", user.Id, "Account locked");
                    return View(model);
                }

                if (signInResult.IsNotAllowed)
                {
                    _logger.LogWarning("User {Email} login not allowed", model.Email);
                    ModelState.AddModelError(string.Empty, "Login not allowed. Contact administrator.");
                    await _audit.LogAsync(user.Id, "Login.NotAllowed", user.Id, "Login not allowed");
                    return View(model);
                }

                if (signInResult.RequiresTwoFactor)
                {
                    _logger.LogInformation("User {Email} requires 2FA", model.Email);
                    ModelState.AddModelError(string.Empty, "Two-factor authentication required.");
                    await _audit.LogAsync(user.Id, "Login.Requires2FA", user.Id, "Requires 2FA");
                    return View(model);
                }

                _logger.LogWarning("Invalid credentials for {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                await _audit.LogAsync(user.Id, "Login.Failed", user.Id, "Invalid credentials");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "Unexpected error. Try again later.");
                return View(model);
            }
        }

        // POST: /Account/SignOut
        [HttpPost]
        [ValidateAntiForgeryToken]
        public new async Task<IActionResult> SignOut()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
                await _audit.LogAsync(user.Id, "SignOut", user.Id, "User signed out");

            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        // GET /Account/ForgetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgetPassword() => View();

        // POST /Account/ForgetPasswordModal
        // Accepts form data (application/x-www-form-urlencoded) including __RequestVerificationToken
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgetPasswordModal(Website.Siegwart.BLL.Dtos.Account.ForgotPasswordDto model)
        {
            if (model == null)
            {
                _logger.LogWarning("ForgetPasswordModal: model is null (empty request body).");
                return BadRequest(new { message = "Invalid request." });
            }

            model.Normalize();

            // Server-side validation
            if (string.IsNullOrWhiteSpace(model.Email) || !new EmailAddressAttribute().IsValid(model.Email))
            {
                _logger.LogInformation("ForgetPasswordModal: invalid email {Email}", model.Email);
                return BadRequest(new { errors = new { Email = new[] { "Please enter a valid email address." } } });
            }

            try
            {
                _logger.LogInformation("ForgetPasswordModal: request for {Email}", model.Email);

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                    var callbackUrl = Url.Action("ResetPassword", "Account", new { email = user.Email, token = encodedToken }, Request.Scheme);

                    var emailMessage = new Email
                    {
                        To = user.Email ?? string.Empty,
                        Subject = "Reset your password",
                        Body = $@"<p>Hello {user.UserName},</p>
                                  <p>To reset your password click the link below:</p>
                                  <p><a href=""{callbackUrl}"">{callbackUrl}</a></p>",
                        IsHtml = true
                    };

                    await _emailSender.SendEmailAsync(emailMessage);
                    await _audit.LogAsync(user.Id, "ForgetPassword.Request", user.Id, "Reset link sent (modal)");
                    _logger.LogInformation("ForgetPasswordModal: reset link sent for {Email}", model.Email);
                }
                else
                {
                    // For security, do not reveal whether the email exists.
                    _logger.LogInformation("ForgetPasswordModal: no user found for {Email}", model.Email);
                }

                // Always reply success message
                return Ok(new { success = true, message = "If an account exists, a reset link has been sent to the email address." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing forget password (modal) for {Email}", model.Email);
                return StatusCode(500, new { message = "An error occurred while processing the request." });
            }
        }

        // GET /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
                return BadRequest();

            var model = new ResetPasswordDto { Email = email, Token = token };
            return View(model);
        }

        // POST /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (!ModelState.IsValid) return View(model);

            model.Normalize();

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid request.");
                return View(model);
            }

            try
            {
                var decoded = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
                var result = await _userManager.ResetPasswordAsync(user, decoded, model.NewPassword);
                if (result.Succeeded)
                {
                    await _audit.LogAsync(user.Id, "ResetPassword.Self", user.Id, "Password reset");
                    TempData["Success"] = "Password reset successfully. You can log in now.";
                    return RedirectToAction(nameof(Login));
                }

                foreach (var err in result.Errors)
                    ModelState.AddModelError(string.Empty, err.Description);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Invalid token for {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "Invalid or expired token.");
            }

            return View(model);
        }

        // GET /Account/SignUp - only SuperAdmin sees the page (form), POST action will be implemented in Admin area
        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public IActionResult SignUp() => View();

        public IActionResult AccessDenied() => View();
    }
}