using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Website.Siegwart.BLL.Services.Interfaces;

namespace Website.Siegwart.PL.Controllers
{

    [Route("team")]
    public class UserTeamController : Controller
    {
        private readonly IUserTeamMemberService _teamMemberService;
        private readonly ILogger<UserTeamController> _logger;

        public UserTeamController(
            IUserTeamMemberService teamMemberService,
            ILogger<UserTeamController> logger)
        {
            _teamMemberService = teamMemberService;
            _logger = logger;
        }

        /// <summary>
        /// Display all active team members
        /// </summary>
        [HttpGet("")]
        [HttpGet("index")]
        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogDebug("Loading team page");

                var teamMembers = await _teamMemberService.GetActiveTeamMembersAsync();

                _logger.LogInformation("Team page loaded with {Count} members", teamMembers.Count);

                return View(teamMembers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading team members page");
                return View(new List<BLL.Dtos.User.UserTeamMemberListDto>());
            }
        }

        /// <summary>
        /// Display team member details
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid team member ID: {Id}", id);
                return NotFound();
            }

            try
            {
                _logger.LogDebug("Loading team member details: {Id}", id);

                var member = await _teamMemberService.GetTeamMemberDetailsAsync(id);

                if (member == null)
                {
                    _logger.LogWarning("Team member not found: {Id}", id);
                    return NotFound();
                }

                _logger.LogInformation("Team member details loaded: {Id} - {Name}", id, member.NameEn);

                return View(member);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading team member details: {Id}", id);
                return NotFound();
            }
        }
    }
}