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
            if (value != 0)
            {
                HttpContext.Session.SetString("Email", user.Email);

                if (value == 1)
                {
                    return RedirectToAction("Dashboard");
                }
                else if (value == 2)
                {
                    return RedirectToAction("ManagerDashboard");
                }
                else if (value == 3)
                {
                    return RedirectToAction("MemberDashboard");
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

        public IActionResult ManagerDashboard()
        {
            var sessiondata = HttpContext.Session.GetString("Email");
            if (sessiondata != null)
            {
                List<UserModel> users = _authService.GetEmployeesByManager(sessiondata);
                return View(users);
            }
            else
            {
                TempData["ToastrMessage"] = "Sign in first!";
                TempData["ToastrType"] = "warning";
                return RedirectToAction("SignIn");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
