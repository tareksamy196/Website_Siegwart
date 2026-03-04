using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Website.Siegwart.PL.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    [Route("admin/[controller]")]
    public abstract class BaseAdminController : Controller
    {
        #region Alert Messages (New System)
        protected void SetAlert(string message, string type = "info")
        {
            TempData["Alert.Message"] = message;
            TempData["Alert.Type"] = type;
        }
        protected void SetSuccessMessage(string message)
        {
            SetAlert(message, "success");
        }
        protected void SetErrorMessage(string message)
        {
            SetAlert(message, "danger");
        }
        protected void SetInfoMessage(string message)
        {
            SetAlert(message, "info");
        }
        protected void SetWarningMessage(string message)
        {
            SetAlert(message, "warning");
        }
        #endregion

        #region Exception Handling
        protected IActionResult HandleException(
            Exception ex,
            ILogger logger,
            string action,
            bool isDevelopment)
        {
            logger.LogError(ex, "Error in {Action}", action);

            var errorMessage = isDevelopment
                ? $"Error: {ex.Message}"
                : "An error occurred. Please try again.";

            SetErrorMessage(errorMessage);

            return RedirectToAction("Index");
        }
        #endregion
        protected string GetCurrentUserEmail()
        {
            var emailClaim = User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            return !string.IsNullOrWhiteSpace(emailClaim) ? emailClaim : (User.Identity?.Name ?? "Unknown");
        }

        protected bool IsInRole(string roleName)
        {
            return User.IsInRole(roleName);
        }
    }
}