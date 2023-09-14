using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pink_Panthers_Project.Data;

namespace Pink_Panthers_Project.Controllers
{
    public class ProfileController : Controller
    {
        private readonly Pink_Panthers_ProjectContext _context;
        public ProfileController(Pink_Panthers_ProjectContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null || _context.Account == null) //So they can't manually type in the url and get an account's info
            {
                return NotFound();
            }
            var account = await _context.Account
                .FirstOrDefaultAsync(m => m.ID == id); //Gets the account with the passed in id, if it exists
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }
    }
}
