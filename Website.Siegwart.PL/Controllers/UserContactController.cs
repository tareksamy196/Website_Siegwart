using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Website.Siegwart.BLL.Dtos.User;
using Website.Siegwart.PL.Services;

namespace Website.Siegwart.PL.Controllers
{
    public class UserContactController : Controller
    {
        private readonly IContactService _contactService;

        public UserContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new UserContactFormDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(UserContactFormDto vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var ua = Request.Headers["User-Agent"].ToString();

            await _contactService.SaveAsync(vm, ip, ua);

            TempData["ContactSuccess"] = true;
            return RedirectToAction(nameof(Thanks));
        }

        [HttpGet]
        public IActionResult Thanks()
        {
            if (TempData["ContactSuccess"] == null)
                return RedirectToAction(nameof(Index));

            return View();
        }
    }
}