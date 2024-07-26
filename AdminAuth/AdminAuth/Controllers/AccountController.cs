using Microsoft.AspNetCore.Mvc;
using Admin.Core.Models;

namespace AdminAuth.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(UserModel user)
        {

            return RedirectToAction("Dashboard", "Home");
        }
    }
}
