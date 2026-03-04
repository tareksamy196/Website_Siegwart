using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Website.Siegwart.DAL.Data.Contexts;

namespace Website.Siegwart.PL.Controllers
{
  
    [Authorize(Roles = "Admin")]
    [Route("admin")]
    [Route("admin/dashboard")]
    public class AdminDashboardController : BaseAdminController 
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AdminDashboardController> _logger;
        private readonly IWebHostEnvironment _env;

        public AdminDashboardController(
            AppDbContext context,
            ILogger<AdminDashboardController> logger,
            IWebHostEnvironment env)
        {
            _context = context;
            _logger = logger;
            _env = env;
        }

        /// <summary>
        /// GET: /admin or /admin/dashboard
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var userName = GetCurrentUserEmail();
                _logger.LogInformation("Loading dashboard for user: {UserName}", userName);

                var stats = new DashboardStatsViewModel
                {
                    // Categories
                    TotalCategories = await _context.Categories.CountAsync(c => !c.IsDeleted),

                    // Products
                    TotalProducts = await _context.Products.CountAsync(p => !p.IsDeleted),
                    ActiveProducts = await _context.Products.CountAsync(p => !p.IsDeleted && p.IsActive),
                    InactiveProducts = await _context.Products.CountAsync(p => !p.IsDeleted && !p.IsActive),

                    // News
                    TotalNews = await _context.News.CountAsync(n => !n.IsDeleted),
                    PublishedNews = await _context.News.CountAsync(n => !n.IsDeleted && n.IsPublished),
                    DraftNews = await _context.News.CountAsync(n => !n.IsDeleted && !n.IsPublished),

                    // Team Members
                    TotalTeamMembers = await _context.TeamMembers.CountAsync(t => !t.IsDeleted),
                    ActiveTeamMembers = await _context.TeamMembers.CountAsync(t => !t.IsDeleted && t.IsActive),
                    InactiveTeamMembers = await _context.TeamMembers.CountAsync(t => !t.IsDeleted && !t.IsActive),

                    // Recent Items
                    RecentProducts = await _context.Products
                        .Where(p => !p.IsDeleted)
                        .OrderByDescending(p => p.CreatedOn)
                        .Take(5)
                        .Select(p => new RecentItemViewModel
                        {
                            Id = p.Id,
                            Title = p.TitleEn,
                            CreatedOn = p.CreatedOn,
                            IsActive = p.IsActive
                        })
                        .ToListAsync(),

                    RecentNews = await _context.News
                        .Where(n => !n.IsDeleted)
                        .OrderByDescending(n => n.CreatedOn)
                        .Take(5)
                        .Select(n => new RecentItemViewModel
                        {
                            Id = n.Id,
                            Title = n.TitleEn,
                            CreatedOn = n.CreatedOn,
                            IsActive = n.IsPublished
                        })
                        .ToListAsync()
                };

                return View(stats);
            }
            catch (Exception ex)
            {
                return HandleException(ex, _logger, nameof(Index), _env.IsDevelopment()); // ⚡ Use base method
            }
        }
    }

    #region View Models
    public class DashboardStatsViewModel
    {
        public int TotalCategories { get; set; }
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int InactiveProducts { get; set; }
        public int TotalNews { get; set; }
        public int PublishedNews { get; set; }
        public int DraftNews { get; set; }
        public int TotalTeamMembers { get; set; }
        public int ActiveTeamMembers { get; set; }
        public int InactiveTeamMembers { get; set; }
        public List<RecentItemViewModel> RecentProducts { get; set; } = new();
        public List<RecentItemViewModel> RecentNews { get; set; } = new();
    }

    public class RecentItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public bool IsActive { get; set; }
    }
    #endregion
}