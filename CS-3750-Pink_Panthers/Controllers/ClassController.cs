using Microsoft.AspNetCore.Mvc;
using Pink_Panthers_Project.Data;
using Pink_Panthers_Project.Models;
using Pink_Panthers_Project.Util;

namespace Pink_Panthers_Project.Controllers
{
    public class ClassController : Controller
    {
        private readonly Pink_Panthers_ProjectContext _context;


        public ClassController(Pink_Panthers_ProjectContext context)
        {
            _context = context;
        }



        public IActionResult Index(int id)
        {
            var account = HttpContext.Session.GetSessionValue<Account>("LoggedInAccount");
            ViewBag.isTeacher = account!.isTeacher;


            var assignments = _context.Assignments.Where(a => a.ClassID == id);
            var currentClass = _context.Class.Find(id);
            var viewModel = new ClassViewModel
            {
                Class = currentClass,
                Assignments = assignments.ToList(),
                Account = account
            };
            return View(viewModel);
        }
    }
}
