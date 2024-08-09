using Microsoft.AspNetCore.Mvc;
using AdminAuth.Models;
using Newtonsoft.Json;
using System.Text;

namespace AdminAuth.Controllers
{
    public class AccountController : Controller
    {
        #region Sign Up
        /// <summary>
        /// Sign Up
        /// </summary>
        /// <returns></returns>
        //[Route("Register")]
        [HttpGet]
        public async Task<ActionResult> SignUp()
        {
            List<List<string>> details = new List<List<string>>();

            using (HttpClient client = new HttpClient())
            {
                string requestUrl = $"https://localhost:7122/api/AccountAPI/details";
                var response = await client.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    details = JsonConvert.DeserializeObject<List<List<string>>>(responseString);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            TempData["Roles"] = details[0];
            TempData["Managers"] = details[1];
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
        public async Task<ActionResult> SignUp(UserModel user)
        {
            using (HttpClient client = new HttpClient())
            {
                string requestUrl = $"https://localhost:7122/api/AccountAPI/signup";
                var jsonContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

                var response = await client.PostAsync(requestUrl, jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    //var responseString = await response.Content.ReadAsStringAsync();
                    //var responseData = JsonConvert.DeserializeObject<dynamic>(responseString);
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
        public async Task<ActionResult> SignIn(UserModel user)
        {
            List<string> details = new List<string>();
            using (HttpClient client = new HttpClient())
            {
                string requestUrl = $"https://localhost:7122/api/AccountAPI/signin";
                var jsonContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(requestUrl, jsonContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    details = JsonConvert.DeserializeObject<List<string>>(responseString);
                    if (details.Count != 0 && user.Email != null)
                    {
                        HttpContext.Session.SetString("Email", user.Email);
                        HttpContext.Session.SetString("Role", details[1]);
                        return RedirectToAction("Dashboard");
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
        }
        #endregion

        #region Employee Dashboard
        /// <summary>
        /// Dashboard
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Dashboard()
        {
            List<UserModel> users = new List<UserModel>();
            var sessionemail = HttpContext.Session.GetString("Email");
            var sessionrole = HttpContext.Session.GetString("Role");

            if (sessionemail != null && sessionrole != null)
            {
                using (HttpClient client = new HttpClient())
                {
                    string requestUrl = $"https://localhost:7122/api/AccountAPI/userdetails?Email={sessionemail}&Role={sessionrole}";
                    var response = await client.GetAsync(requestUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        users = JsonConvert.DeserializeObject<List<UserModel>>(responseString);
                        TempData["employeeCount"] = users.Count - 1;
                    }
                    else
                    {
                        TempData["ToastrMessage"] = "Denied!";
                        TempData["ToastrType"] = "error";
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            else
            {
                TempData["ToastrMessage"] = "Denied!";
                TempData["ToastrType"] = "error";
                return RedirectToAction("Index", "Home");
            }
            return View(users);
        }
        #endregion

        #region Edit Employee
        /// <summary>
        /// Edit Employee
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        //[Route("Edit")]
        [HttpGet]
        public async Task<ActionResult> EditEmployee(string Email)
        {
            UserModel user = new UserModel();

            using (HttpClient client = new HttpClient())
            {
                string requestUrl = $"https://localhost:7122/api/AccountAPI/getuser?Email={Email}";
                var response = await client.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    user = JsonConvert.DeserializeObject<UserModel>(responseString);
                }
                else
                {
                    return RedirectToAction("Dashboard");
                }
            }
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
        public async Task<ActionResult> Update(UserModel user)
        {
            using (HttpClient client = new HttpClient())
            {
                string requestUrl = $"https://localhost:7122/api/AccountAPI/update";
                var jsonContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

                var response = await client.PutAsync(requestUrl, jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var responseData = JsonConvert.DeserializeObject<dynamic>(responseString);
                    TempData["ToastrMessage"] = $"Employee {user.Name} updated successfully!";
                    TempData["ToastrType"] = "success";
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    TempData["ToastrMessage"] = "Failed to update employee.";
                    TempData["ToastrType"] = "error";
                    return RedirectToAction("EditEmployee", user);
                }
            }
        }
        #endregion

        #region Soft delete employee by email
        /// <summary>
        /// Soft delete employee by email
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> DeleteEmployee(string Email)
        {
            using (HttpClient client = new HttpClient())
            {
                string requestUrl = $"https://localhost:7122/api/AccountAPI/delete?Email={Email}";
                var jsonContent = new StringContent(JsonConvert.SerializeObject(Email), Encoding.UTF8, "application/json");

                var response = await client.PutAsync(requestUrl, jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["ToastrMessage"] = "Employee deleted successfully!";
                    TempData["ToastrType"] = "warning";
                    //var responseString = await response.Content.ReadAsStringAsync();
                    //var responseData = JsonConvert.DeserializeObject<dynamic>(responseString);
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    TempData["ToastrMessage"] = "Failed to delete employee.";
                    TempData["ToastrType"] = "error";
                    return RedirectToAction("Dashbboard");
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
