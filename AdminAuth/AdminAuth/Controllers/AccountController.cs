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

        #region Sign Up
        /// <summary>
        /// Sign Up
        /// </summary>
        /// <returns></returns>
        //[Route("Register")]
        public IActionResult SignUp()
        {
            HttpContext.Session.Clear();
            TempData["Roles"] = _authService.GetRoles();
            TempData["Managers"] = _authService.GetManagers();
            return View();
        }
        #endregion

        #region Sign Up Post Action
        /// <summary>
        /// Sign Up Post Action
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SignUp(UserModel user)
        {
            if (_authService.SignUp(user))
            {
                TempData["ToastrMessage"] = "Signed up successfully. You can sign in.";
                TempData["ToastrType"] = "success";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["ToastrMessage"] = "Email already exists. Please Sign In";
                TempData["ToastrType"] = "warning";
                return RedirectToAction("SignUp", user);
            }           
        }
        #endregion

        #region Sign In
        /// <summary>
        /// Sign In
        /// </summary>
        /// <returns></returns>
        //[Route("Login")]
        public IActionResult SignIn()
        {
            return View();
        }
        #endregion

        #region Sign In Post Action
        /// <summary>
        /// Sign In Post Action
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SignIn(UserModel user)
        {
            var value = _authService.SignIn(user);
            if (value.Count != 0)
            {
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("Role", value[1]);
                if (value[1] == "Admin")
                {
                    TempData["ToastrMessage"] = $"Hi {value[0]}";
                    TempData["ToastrType"] = "success";
                    return RedirectToAction("AdminDashboard");
                }
                else if (value[1] == "Manager")
                {
                    TempData["ToastrMessage"] = $"Hi {value[0]}";
                    TempData["ToastrType"] = "success";
                    return RedirectToAction("ManagerDashboard");
                }
                else if (value[1] == "Team Member")
                {
                    TempData["ToastrMessage"] = $"Hi {value[0]}";
                    TempData["ToastrType"] = "success";
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
        #endregion

        #region Dashboard for Admin
        /// <summary>
        /// Dashboard for Admin
        /// </summary>
        /// <returns></returns>
        //[Route("Dashboard")]
        public IActionResult AdminDashboard()
        {
            var sessionemail = HttpContext.Session.GetString("Email");
            var sessionrole = HttpContext.Session.GetString("Role");
            if (sessionemail != null && sessionrole == "Admin")
            {
                List<UserModel> users = _authService.GetEmployeesForAdmin(sessionemail);
                TempData["employeeCount"] = users.Count - 1;
                return View(users);
            }
            else
            {
                TempData["ToastrMessage"] = "Sign in as Admin!";
                TempData["ToastrType"] = "warning";
                return RedirectToAction("SignIn");
            }
        }
        #endregion

        #region Dashboard for Manager
        /// <summary>
        /// Dashboard for Manager
        /// </summary>
        /// <returns></returns>
        //[Route("Dashboard")]
        public IActionResult ManagerDashboard()
        {
            var sessionemail = HttpContext.Session.GetString("Email");
            var sessionrole = HttpContext.Session.GetString("Role");
            if (sessionemail != null && sessionrole == "Manager")
            {
                List<UserModel> users = _authService.GetEmployeesByManager(sessionemail);
                TempData["employeeCount"] = users.Count - 1;
                return View(users);
            }
            else
            {
                TempData["ToastrMessage"] = "Sign in as Manager!";
                TempData["ToastrType"] = "warning";
                return RedirectToAction("SignIn");
            }
        }
        #endregion

        #region Dashboard for User
        /// <summary>
        /// Dashboard for User
        /// </summary>
        /// <returns></returns>
        //[Route("Dashboard")]
        public IActionResult UserDashboard()
        {
            var sessionemail = HttpContext.Session.GetString("Email");
            var sessionrole = HttpContext.Session.GetString("Role");
            if (sessionemail != null && sessionrole == "Team Member")
            {
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
        #endregion

        #region Edit Employee
        /// <summary>
        /// Edit Employee
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        //[Route("Edit")]
        public IActionResult EditEmployee(string Email)
        {
            TempData["Roles"] = _authService.GetRoles();
            TempData["Managers"] = _authService.GetManagers();
            UserModel user = _authService.EditEmployee(Email);
            return View("EditEmployee", user);
        }
        #endregion

        #region Update Employee Post Action
        /// <summary>
        /// Update Employee Post Action
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
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
        #endregion

        #region Soft delete employee by email
        /// <summary>
        /// Soft delete employee by email
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public IActionResult DeleteEmployee(string Email)
        {
            var sessionrole = HttpContext.Session.GetString("Role");
            if (_authService.DeleteEmployee(Email))
            {
                TempData["ToastrMessage"] = "Employee deleted successfully!";
                TempData["ToastrType"] = "warning";
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
                TempData["ToastrMessage"] = "Failed to delete employee.";
                TempData["ToastrType"] = "error";
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

        }
        #endregion

        #region Log out
        /// <summary>
        /// Log out
        /// </summary>
        /// <returns></returns>
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["ToastrMessage"] = "Logged out successfully!";
            TempData["ToastrType"] = "success";
            return RedirectToAction("Index", "Home");
        }
        #endregion
    }
}
