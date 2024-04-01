using Microsoft.AspNetCore.Mvc;

namespace TimeShareProject.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult ErrorNotFound()
        {
            // You can customize the 404 view here
            return View("ErrorNotFound");
        }
    }
}
