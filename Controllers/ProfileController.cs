using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pink_Panthers_Project.Data;
using Pink_Panthers_Project.Models;

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
        public IActionResult Index(Account? account = null)
        {
            if (account != null)
            {
                if (isValidState(ref account))
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
        private bool isValidState(ref Account account)
        {
            if(account.ID != null && account.Email != null && account.Password != null 
                && account.ConfirmPassword != null && account.FirstName != null 
                && account.LastName != null && account.Salt != null)
            {
                return true;
            }
            return false;
        }
    }
}
