using ASP_P42.Models;
using ASP_P42.Services.Hash;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ASP_P42.Controllers
{
    public class HomeController(IHashServices hashService) : Controller
    {
        private readonly IHashServices _hashService = hashService;

        public IActionResult IoC()
        {
            String digest = _hashService.Digest("geralt roger");
            ViewBag.Hash = _hashService.GetHashCode();
            ViewData["digest"] = digest;

            return View();
        }

        public IActionResult Razor()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Intro()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
