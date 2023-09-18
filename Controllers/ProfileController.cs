using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using Pink_Panthers_Project.Data;
using Pink_Panthers_Project.Models;
using System.Text;
using System.Web;

namespace Pink_Panthers_Project.Controllers
{
    public class ProfileController : Controller
    {
        private readonly Pink_Panthers_ProjectContext _context;
        public ProfileController(Pink_Panthers_ProjectContext context)
        {
            _context = context;
        }
        private static Account? _account;

        public IActionResult Index()
        {
            if(_account != null) //An account must be active to view this page
            {
                return View(_account);
            }
            return NotFound();
        }

        public IActionResult Privacy()
        {
            if(_account != null)
                return View();
            return NotFound();
        }

        public static Account? getAccount() //Returns the account if it's not null
        {
            if (_account != null)
            {
                return _account;
            }
            return null;
        }

        public static void setAccount(ref Account account) //Used to set the current account
        {
            _account = account;
        }

        public static void logoutAccount() //Sets the current account to null
        {
            _account = null;
        }
    }
}
