using Microsoft.AspNetCore.Mvc;
using Admin.Core.Models;
using Admin.Core.IServices;

namespace AdminAuth.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }
        public IActionResult SignUp()
        {
            HttpContext.Session.Clear();
            TempData["Managers"] = _authService.GetManagers();
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(UserModel user)
        {
            _authService.SignUp(user);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignIn(UserModel user)
        {
            var value = _authService.SignIn(user);
            if (value != "_")
            {
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("Role", value);
                if (value == "Admin")
                {
                    return RedirectToAction("AdminDashboard");
                }
                else if (value == "Manager")
                {
                    return RedirectToAction("ManagerDashboard");
                }
                else if (value == "Team Member")
                {
                    return RedirectToAction("UserDashboard");
                }
                else
                {
                    TempData["ToastrMessage"] = "Sign in failed!";
                    TempData["ToastrType"] = "error";
                    return View();
                }
            }
            else
            {
                TempData["ToastrMessage"] = "Sign in failed!";
                TempData["ToastrType"] = "error";
                return View();
            }
        }

        public IActionResult AdminDashboard()
        {
            var sessionemail = HttpContext.Session.GetString("Email");
            var sessionrole = HttpContext.Session.GetString("Role");
            if (sessionemail != null && sessionrole == "Admin")
            {
                TempData["ToastrMessage"] = "Hi Admin!";
                TempData["ToastrType"] = "success";
                List<UserModel> users = _authService.GetEmployeesForAdmin(sessionemail);
                return View(users);
            }
            else
            {
                TempData["ToastrMessage"] = "Sign in as Admin!";
                TempData["ToastrType"] = "warning";
                return RedirectToAction("SignIn");
            }
        }

        public IActionResult ManagerDashboard()
        {
            var sessionemail = HttpContext.Session.GetString("Email");
            var sessionrole = HttpContext.Session.GetString("Role");
            if (sessionemail != null && sessionrole == "Manager")
            {
                TempData["ToastrMessage"] = "Hi Manager!";
                TempData["ToastrType"] = "success";
                List<UserModel> users = _authService.GetEmployeesByManager(sessionemail);
                return View(users);
            }
            else
            {
                TempData["ToastrMessage"] = "Sign in as Manager!";
                TempData["ToastrType"] = "warning";
                return RedirectToAction("SignIn");
            }
        }

        public IActionResult UserDashboard()
        {
            var sessionemail = HttpContext.Session.GetString("Email");
            var sessionrole = HttpContext.Session.GetString("Role");
            if (sessionemail != null && sessionrole == "Team Member")
            {
                TempData["ToastrMessage"] = "Hi User!";
                TempData["ToastrType"] = "success";
                UserModel users = _authService.GetEmployeeByEmail(sessionemail);
                return View(users);
            }
            else
            {
                TempData["ToastrMessage"] = "Sign in first!";
                TempData["ToastrType"] = "warning";
                return RedirectToAction("SignIn");
            }
        }

        public IActionResult EditEmployee(string Email)
        {
            TempData["Managers"] = _authService.GetManagers();
            UserModel user = _authService.EditEmployee(Email);
            return View("EditEmployee", user);
        }

        [HttpPost]
        public IActionResult Update(UserModel user)
        {
            var sessionrole = HttpContext.Session.GetString("Role");

            if (_authService.UpdateEmployee(user))
            {
                TempData["ToastrMessage"] = $"Employee {user.Name} updated successfully!";
                TempData["ToastrType"] = "success";
                if (sessionrole == "Admin")
                {
                    return RedirectToAction("AdminDashboard");
                }
                else if (sessionrole == "Manager")
                {
                    return RedirectToAction("ManagerDashboard");
                }
                else
                {
                    return RedirectToAction("UserDashboard");
                }
            }
            else
            {
                TempData["ToastrMessage"] = "Failed to update employee.";
                TempData["ToastrType"] = "error";
                return RedirectToAction("EditEmployee");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
