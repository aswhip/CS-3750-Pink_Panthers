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
            if (id == null || _context.Account == null)
            {
                return NotFound();
            }
            var account = await _context.Account
                .FirstOrDefaultAsync(m => m.ID == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }
    }
}
