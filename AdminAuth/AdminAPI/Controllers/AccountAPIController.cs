using Admin.Core.IServices;
using Admin.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AccountAPIController : ControllerBase
    {

        #region Declarations

        private readonly ILogger<AccountAPIController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthService _authService;

        #endregion

        #region Constructor

        public AccountAPIController(ILogger<AccountAPIController> logger, IHttpContextAccessor httpContextAccessor, IAuthService authService)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _authService = authService;
        }

        #endregion

        #region Get roles and managers for sign up
        /// <summary>
        /// Get roles for sign up
        /// </summary>
        /// <returns></returns>
        [Route("details")]
        [HttpGet]
        public List<List<string>> GetDetails()
        {
            return _authService.GetDetails();
        }
        #endregion

        #region Sign Up
        /// <summary>
        /// Sign Up
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Route("signup")]
        [HttpPost]
        public bool SignUp(UserModel user)
        {
            return _authService.SignUp(user);
        }
        #endregion

        #region Sign In
        /// <summary>
        /// Sign In
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Route("signin")]
        [HttpPost]
        public List<string> SignIn(UserModel user)
        {
            return _authService.SignIn(user);
        }
        #endregion

        #region Dashboard
        /// <summary>
        /// Dashboard
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="Role"></param>
        /// <returns></returns>
        [Route("userdetails")]
        [HttpGet]
        public List<UserModel> UserDetails(string Email, string Role)
        {
            List<UserModel> users = new List<UserModel>();
            if (Role == "Admin")
            {
                users = _authService.GetEmployeesForAdmin(Email);
            }
            else if (Role == "Manager")
            {
                users = _authService.GetEmployeesByManager(Email);
            }
            else if (Role == "Team Member")
            {
                var user = _authService.GetEmployeeByEmail(Email);
                users.Add(user);
            }
            else
            {
                return new List<UserModel>();
            }
            return users;
        }
        #endregion

        #region Get user
        /// <summary>
        /// Edit Employee
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        [Route("getuser")]
        [HttpGet]
        public UserModel GetUser(string Email)
        {
            return _authService.EditEmployee(Email);
        }
        #endregion

        #region Update Employee
        /// <summary>
        /// Update Employee
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Route("update")]
        [HttpPut]
        public bool Update(UserModel user)
        {
            return _authService.UpdateEmployee(user);
        }
        #endregion

        #region Soft delete by Email
        [Route("delete")]
        [HttpPut]
        public bool Delete(string Email)
        {
            return _authService.DeleteEmployee(Email);
        }
        #endregion
    }

}
