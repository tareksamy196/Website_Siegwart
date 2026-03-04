using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Website.Siegwart.BLL.Dtos.Admin.VideoMedia;
using Website.Siegwart.BLL.Services.Interfaces;

namespace Website.Siegwart.PL.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("admin/video")]
    public class AdminVideoMediaController : Controller
    {
        private readonly IVideoMediaService _service;

        public AdminVideoMediaController(IVideoMediaService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        // GET /admin/video
        [HttpGet("")]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 20)
        {
            var (total, items) = await _service.GetPagedAsync(page, pageSize);
            ViewBag.TotalItems = total;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            return View(items);
        }

        // GET /admin/video/create
        [HttpGet("create")]
        public IActionResult Create() => View();

        // POST /admin/video/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VideoMediaCreateDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            try
            {
                await _service.CreateAsync(dto);
                TempData["Success"] = "Video added successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        }

        // GET /admin/video/edit/{id}
        [HttpGet("edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();

            var updateDto = new VideoMediaUpdateDto
            {
                Id = dto.Id,
                VideoUrl = dto.SourceUrl ?? dto.VideoId,
                TitleEn = dto.TitleEn ?? string.Empty,
                TitleAr = dto.TitleAr ?? string.Empty,
                DescriptionEn = dto.DescriptionEn,
                DescriptionAr = dto.DescriptionAr,
                IsPublished = dto.IsPublished,
                SortOrder = dto.SortOrder
            };

            return View(updateDto);
        }

        // POST /admin/video/edit/{id}
        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VideoMediaUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest();
            if (!ModelState.IsValid) return View(dto);

            try
            {
                await _service.UpdateAsync(dto);
                TempData["Success"] = "Video updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        }

        //// GET /admin/video/details/{id}
        //[HttpGet("details/{id:int}")]
        //public async Task<IActionResult> Details(int id)
        //{
        //    var dto = await _service.GetByIdAsync(id);
        //    if (dto == null) return NotFound();
        //    return View(dto);
        //}

        // POST /admin/video/delete/{id}
        [HttpPost("delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                TempData["Success"] = "Video removed.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Unable to delete video.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST /admin/video/togglepublish/{id}
        [HttpPost("togglepublish/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePublish(int id, [FromForm] bool publish)
        {
            try
            {
                await _service.TogglePublishAsync(id, publish);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Unable to change publish state.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}