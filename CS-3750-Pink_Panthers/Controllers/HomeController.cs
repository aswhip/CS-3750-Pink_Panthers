﻿using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public IActionResult Index()
        {
            if(ProfileController.getAccount() != null) //Only take us to the profile page if the profile account is active
            {
                return RedirectToAction(nameof(ProfileController.Index), "Profile");
            }
            return RedirectToAction("Login", "Accounts");//Redirects to the login page on load
        }

        public IActionResult Logout()
        {
            if (ProfileController.getAccount() != null) //Can only log out if user is currently logged in
            {
                ProfileController.logoutAccount();
                HttpContext.Session.Remove("LoggedInAccount"); //Remove account from session
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