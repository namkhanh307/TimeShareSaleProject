using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TimeShareProject.Models;

namespace TimeShareProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;


        public IActionResult Index()
        {
            using _4restContext context = new _4restContext();
            var items = context.Projects.OrderBy(p => p.Name).ToList();
            return View(items);
        }

        public IActionResult ViewDashBoard()
        {
            return View();
        }

        public IActionResult ContactUs()
        {
            return View();
        }
        public IActionResult TermsAndPolicies()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ContactUs(Contact contact)
        {
            using _4restContext _context = new _4restContext();
            if (ModelState.IsValid)
            {




                _context.Contacts.Add(contact);
                _context.SaveChanges();

                return RedirectToAction("ContactUs");
            }
            return View(contact);
        }
        [Authorize(Roles = "1")]
        public IActionResult AdminHome()
        {
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "2")]
        public IActionResult StaffHome()
        {
            return RedirectToAction("Index", "Home");
        }
        [Authorize(Roles = "3")]
        public IActionResult MemberHome()
        {
            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
