using Microsoft.AspNetCore.Mvc;
using Pink_Panthers_Project.Models;
using System.Diagnostics;

namespace Pink_Panthers_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        private Account? account = null;

        [HttpGet]
        public IActionResult Index()
        {
            account = ProfileController.getAccount();
            if(account != null)
            {
                return RedirectToAction(nameof(ProfileController.Index), "Profile", account);
            }
            return RedirectToAction("Login", "Accounts", account);//Redirects to the login page on load
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Logout()
        {
            account = ProfileController.getAccount();
            if (account != null)
            {
                ProfileController.logoutAccount();
                return View();
            }
            return NotFound();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}