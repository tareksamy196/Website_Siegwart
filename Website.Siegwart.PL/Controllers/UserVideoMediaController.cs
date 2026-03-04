using Microsoft.AspNetCore.Mvc;
using Website.Siegwart.BLL.Services.Interfaces;

namespace Website.Siegwart.PL.Controllers
{
    public class UserVideoMediaController : Controller
    {
        private readonly IVideoMediaService _service;

        public UserVideoMediaController(IVideoMediaService service)
        {
            _service = service;
        }
        public async Task<IActionResult> Index()
        {
            var items = await _service.GetPublishedAsync(take: 24);
            return View(items);
        }
        public async Task<IActionResult> Details(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return View(dto);
        }
    }
}