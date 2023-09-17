using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public IActionResult Index(Account? account)
        {
            if (account != null)
            {
				if (_context.Account.SingleOrDefault(m => m.Email == account.Email && m.Password == account.Password) != null)
                {
					_account = account;
                    return View(account);
                }
            }
            return NotFound();
        }

        

        public IActionResult Privacy()
        {
            if(_account != null)
                return View();
            return NotFound();
        }

        public static Account? getAccount()
        {
            if (_account != null)
            {
                return _account;
            }
            return null;
        }

        public static void logoutAccount()
        {
            _account = null;
        }
    }
}
