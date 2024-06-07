using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;

namespace AccountingSystem.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            var redirectUrl = Url.Action(nameof(Index), "Home");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, "Auth0");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            var callbackUrl = Url.Action(nameof(Index), "Home", values: null, protocol: Request.Scheme);
            await HttpContext.SignOutAsync("Auth0", new AuthenticationProperties { RedirectUri = callbackUrl });
            await HttpContext.SignOutAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Index));
        }
    }
}
