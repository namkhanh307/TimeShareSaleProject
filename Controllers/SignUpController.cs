using Microsoft.AspNetCore.Mvc;
using TimeShareProject.Models;

namespace TimeShareProject.Controllers
{
    public class SignUpController : Controller
    {
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(string username, string password, string name, string phone, bool sex, DateTime dateOfBirth, string email)
        {
            _4restContext _dbContext = new _4restContext();

            // Check if the username already exists
            var existingAccount = _dbContext.Accounts.FirstOrDefault(a => a.Username == username);
            if (existingAccount != null)
            {
                TempData["errorExistUsername"] = "Username already exists!";
                return RedirectToAction("SignUp");
            }
            if(username.Contains(" "))
            {
                TempData["errorSpace"] = "User name can not contain space!";
                return RedirectToAction("SignUp");
            }
            if(dateOfBirth >= DateTime.Today)
            {
                TempData["errorBirthday"] = "Birthday can not exceed today!";
                return RedirectToAction("SignUp");
            }
            // Create and save the account
            Account account = new Account { Username = username, Password = password, Role = 3 };
            _dbContext.Accounts.Add(account);
            _dbContext.SaveChanges();


            Account acc = _dbContext.Accounts.First(a => a.Username == username);
            if (acc != null)
            {
                // Create and save the user
                User user = new User { Email = email, Name = name, PhoneNumber = phone, DateOfBirth = dateOfBirth, Sex = sex, AccountId = acc.Id, Status = true };
                _dbContext.Users.Add(user);
                _dbContext.SaveChanges();

                return RedirectToAction("Login", "Login");
            }
            else
            {
                TempData["error"] = "Failed to create account.";
                return RedirectToAction("SignUp");
            }
        }
    }
}
