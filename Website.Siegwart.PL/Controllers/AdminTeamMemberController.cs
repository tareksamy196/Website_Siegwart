using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Website.Siegwart.BLL.Dtos.Admin.TeamMember;
using Website.Siegwart.BLL.Services.Interfaces;
using Website.Siegwart.DAL.Enums;

namespace Website.Siegwart.PL.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("admin/team")]
    public class AdminTeamMemberController : BaseAdminController
    {
        private readonly ITeamMemberService _teamMemberService;
        private readonly ILogger<AdminTeamMemberController> _logger;
        private readonly IWebHostEnvironment _env;

        public AdminTeamMemberController(
            ITeamMemberService teamMemberService,
            ILogger<AdminTeamMemberController> logger,
            IWebHostEnvironment env)
        {
            _teamMemberService = teamMemberService;
            _logger = logger;
            _env = env;
        }

        /// <summary>
        /// Display all team members
        /// </summary>
        [HttpGet("")]
        [HttpGet("index")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var teamMembers = await _teamMemberService.GetAllAsync();
                return View(teamMembers);
            }
            catch (Exception ex)
            {
                return HandleException(ex, _logger, nameof(Index), _env.IsDevelopment());
            }
        }

        /// <summary>
        /// Display create team member form
        /// </summary>
        [HttpGet("create")]
        public IActionResult Create()
        {
            PopulateCategoriesDropdown();
            return View(new CreateTeamMemberDto());
        }

        /// <summary>
        /// Create a new team member
        /// </summary>
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTeamMemberDto model)
        {
            if (!ModelState.IsValid)
            {
                PopulateCategoriesDropdown();
                return View(model);
            }

            try
            {
                await _teamMemberService.CreateAsync(model);

                SetSuccessMessage($"Team member '{model.NameEn}' created successfully.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating team member: {@Model}", model);

                var errorMsg = _env.IsDevelopment()
                    ? $"Error: {ex.Message}"
                    : "An error occurred while creating the team member.";

                ModelState.AddModelError(string.Empty, errorMsg);
                PopulateCategoriesDropdown();
                return View(model);
            }
        }

        /// <summary>
        /// Display edit team member form
        /// </summary>
        [HttpGet("edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var model = await _teamMemberService.GetByIdAsync(id);

                if (model == null)
                {
                    SetErrorMessage("Team member not found.");
                    return RedirectToAction(nameof(Index));
                }

                PopulateCategoriesDropdown();
                return View(model);
            }
            catch (Exception ex)
            {
                return HandleException(ex, _logger, $"{nameof(Edit)} - ID: {id}", _env.IsDevelopment());
            }
        }

        /// <summary>
        /// Update an existing team member
        /// </summary>
        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateTeamMemberDto model)
        {
            if (id != model.Id)
            {
                SetErrorMessage("Invalid request. ID mismatch.");
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                PopulateCategoriesDropdown();
                return View(model);
            }

            try
            {
                await _teamMemberService.UpdateAsync(model);

                SetSuccessMessage($"Team member '{model.NameEn}' updated successfully.");

                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                SetErrorMessage($"Team member with ID {id} not found.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating team member: {@Model}", model);

                var errorMsg = _env.IsDevelopment()
                    ? $"Error: {ex.Message}"
                    : "An error occurred while updating the team member.";

                ModelState.AddModelError(string.Empty, errorMsg);
                PopulateCategoriesDropdown();
                return View(model);
            }
        }

        /// <summary>
        /// Delete a team member
        /// </summary>
        [HttpPost("delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _teamMemberService.DeleteAsync(id);

                SetSuccessMessage("Team member deleted successfully.");
            }
            catch (KeyNotFoundException)
            {
                SetErrorMessage($"Team member with ID {id} not found.");
            }
            catch (Exception ex)
            {
                return HandleException(ex, _logger, $"{nameof(Delete)} - ID: {id}", _env.IsDevelopment());
            }

            return RedirectToAction(nameof(Index));
        }

        #region Helper Methods

        private void PopulateCategoriesDropdown()
        {
            var categories = Enum.GetValues(typeof(TeamCategory))
                .Cast<TeamCategory>()
                .Select(e => new
                {
                    Value = (int)e,
                    Text = GetEnumDisplayName(e)
                })
                .ToList();

            ViewBag.Categories = new SelectList(categories, "Value", "Text");
        }

        private string GetEnumDisplayName(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DisplayAttribute>();
            return attribute?.Name ?? value.ToString();
        }

        #endregion
    }
}