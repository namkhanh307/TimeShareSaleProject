using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimeShareProject.Models;

namespace TimeShareProject.Controllers
{
    public class LoginController : Controller
    {
        private readonly TimeShareProjectContext _dbContext;

        public LoginController(TimeShareProjectContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl; // Pass the ReturnUrl to the view
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password, string returnUrl = null)
        {

            var user = _dbContext.Accounts.SingleOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {

                List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

                // Create identity
                var userIdentity = new ClaimsIdentity(claims, "Login");

                // Create principal
                var userPrincipal = new ClaimsPrincipal(userIdentity);

                // Sign in user
                HttpContext.SignInAsync(userPrincipal);


                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    switch (user.Role)
                    {
                        case 1:
                            return RedirectToAction("AdminHome", "Home");
                        case 2:
                            return RedirectToAction("StaffHome", "Home");
                        case 3:
                            return RedirectToAction("MemberHome", "Home");
                        default:
                            return RedirectToAction("Index", "Home");
                    }
                }
            }
            
            
                TempData["error"] = "Invalid username or password";
            



            return RedirectToAction("Login", "Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
