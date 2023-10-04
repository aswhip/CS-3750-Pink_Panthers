using Microsoft.AspNetCore.Mvc;
using Pink_Panthers_Project.Data;
using Pink_Panthers_Project.Models;

namespace Pink_Panthers_Project.Controllers
{
    public class ClassController : Controller
    {
        private readonly Pink_Panthers_ProjectContext _context;


        public ClassController(Pink_Panthers_ProjectContext context)
        {
            _context = context;
        }

        private static Account? _account;


        public IActionResult Index(int id)
        {
            ViewBag.isTeacher = _account!.isTeacher;


            var assignments = _context.Assignments.Where(a => a.ClassID == id);
            var currentClass = _context.Class.Find(id);
            var viewModel = new ClassViewModel
            {
                Class = currentClass,
                Assignments = assignments.ToList()
            };
            return View(viewModel);
        }
    }
}
