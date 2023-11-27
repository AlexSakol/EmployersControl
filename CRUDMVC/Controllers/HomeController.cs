using CRUDMVC.Models;
using CRUDMVC.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Authorization;

namespace CRUDMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        DataContext context;

        public HomeController(ILogger<HomeController> logger, DataContext dataContext)
        {
            _logger = logger;
            context = dataContext;

        }

        public IActionResult Index() => View();
        
        public async Task<IActionResult> EmployerList()
        {
            if (User.Identity.IsAuthenticated) 
            {
                List<Department> departments = await context.Departments.ToListAsync();
                List<Employer> employers = await context.Employers.ToListAsync();
                var items = employers.Join(departments,
                    e => e.DepartmentId,
                    d => d.Id,
                    (e, d) => new { Id = e.Id, Name = e.Name, Post = e.Post, Salary = e.Salary, DepartmentName = d.Name });
                ViewBag.Items = items;
                return View();
            }
            else return BadRequest("Вы не вошли в систему");
        }
             
        public async Task<IActionResult> Add()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.Departments = await GetDepartments();
                return View();
            }
            else return BadRequest("Вы не вошли в систему");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add(Employer employer)
        {
            if (ModelState.IsValid)
            {
                await context.Employers.AddAsync(employer);
                await context.SaveChangesAsync();
            }
            return RedirectToAction("EmployerList");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            Employer? employer = await context.Employers.FirstOrDefaultAsync(e => e.Id == id);
            if (employer != null) {
                context.Employers.Remove(employer);
                await context.SaveChangesAsync();
            }
            return RedirectToAction("EmployerList");
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            Employer? employer = await context.Employers.FindAsync(id);
            ViewBag.Departments = await GetDepartments();
            if (employer != null) return View(employer);
            else return RedirectToAction("EmployerList");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(Employer employer)
        {
            if (ModelState.IsValid)
            {
                context.Employers.Update(employer);
                await context.SaveChangesAsync();
            }
            return RedirectToAction("EmployerList");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<IEnumerable<SelectListItem>> GetDepartments()
        {
            List<Department> departments = await context.Departments.ToListAsync();
            return departments.Select(department => new SelectListItem
            {                
                Text = department.Name,
                Value = department.Id.ToString(),
            });
        }
    }
}
