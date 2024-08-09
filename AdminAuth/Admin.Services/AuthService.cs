using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Admin.Core.IRepositories;
using Admin.Core.IServices;
using Admin.Core.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Admin.Services
{
    public class AuthService : IAuthService
    {
        #region Declaration

        private readonly IAuthRepository _authRepository;

        #endregion

        #region Constructor

        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        #endregion

        #region Sign Up
        /// <summary>
        /// Sign Up
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool SignUp(UserModel user)
            {
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(password: user.Password!, salt: salt, prf: KeyDerivationPrf.HMACSHA256, iterationCount: 100000, numBytesRequested: 256 / 8));
            user.Salt = salt;
            user.PasswordHash = hashed;
            return _authRepository.SignUp(user);
        }
        #endregion

        #region Checking for duplicate entry
        /// <summary>
        /// Checking for duplicate entry
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool CheckDuplicate(UserModel user)
        {
            return _authRepository.CheckDuplicate(user);
        }
        #endregion

        #region Get roles for sign up
        /// <summary>
        /// Get roles for sign up
        /// </summary>
        /// <returns></returns>
        public List<string> GetRoles()
        {
            List<string> result = new List<string>();
            result = _authRepository.GetRoles();
            return result;
        }
        #endregion

        #region Get managers for sign up
        /// <summary>
        /// Getting managers for sign up
        /// </summary>
        /// <returns></returns>
        public List<string> GetManagers()
        {
            List<string> result = new List<string>();
            result = _authRepository.GetManagers();
            return result;
        }
        #endregion

        #region Get roles and managers for sign up
        /// <summary>
        /// Get details for sign up
        /// </summary>
        /// <returns></returns>
        public List<List<string>> GetDetails()
        {
            return _authRepository.GetDetails();
        }
        #endregion

        #region Sign In
        /// <summary>
        /// Sign In
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<string> SignIn(UserModel user)
        {
            List<string> details = new List<string>();
            var user_creds = _authRepository.GetCreds(user.Email);
            byte[] user_salt = user_creds.Salt;
            if (user_salt != null)
            {              
                string user_hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(password: user.Password!, salt: user_salt, prf: KeyDerivationPrf.HMACSHA256, iterationCount: 100000, numBytesRequested: 256 / 8));
                //Comparing hash values
                if (user_hashed == user_creds.PasswordHash)
                {
                    details.Add(user_creds.Name);
                    details.Add(user_creds.Role);
                    return details;
                }
                else
                {
                    return [];
                }
            }
            else
            {
                return [];
            }
        }
        #endregion

        #region Get an employee by email
        /// <summary>
        /// Get an employee by email
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public UserModel GetEmployeeByEmail(string Email)
        {
            return _authRepository.GetEmployeeByEmail(Email);
        }
        #endregion

        #region Get employees by their manager
        /// <summary>
        /// Get employees by their manager
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public List<UserModel> GetEmployeesByManager(string Email)
        {
            return _authRepository.GetEmployeesByManager(Email);
        }
        #endregion

        #region Get all employees for admin
        /// <summary>
        /// Get all employees for admin
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public List<UserModel> GetEmployeesForAdmin(string Email)
        {
            return _authRepository.GetEmployeesForAdmin(Email);
        }
        #endregion

        #region Edit Employee
        /// <summary>
        /// Edit Employee
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public UserModel EditEmployee(string Email)
        {
            return _authRepository.EditEmployee(Email);
        }
        #endregion

        #region Update Employee
        /// <summary>
        /// Update Employee
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UpdateEmployee(UserModel user)
        {
            return _authRepository.UpdateEmployee(user);
        }
        #endregion

        #region Deleting an employee
        /// <summary>
        /// Deleting an employee
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public bool DeleteEmployee(string Email)
        {
            return _authRepository.DeleteEmployee(Email);
        }
        #endregion
    }
}
