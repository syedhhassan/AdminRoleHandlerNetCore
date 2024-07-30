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
        private readonly IAuthRepository _authRepository;

        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

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

        public bool CheckDuplicate(UserModel user)
        {
            return _authRepository.CheckDuplicate(user);
        }

        public List<string> GetManagers()
        {
            List<string> result = new List<string>();
            result = _authRepository.GetManagers();
            return result;
        }

        public string SignIn(UserModel user)
        {
            var user_creds = _authRepository.GetCreds(user.Email);
            byte[] user_salt = user_creds.Salt;
            var user_role = user_creds.Role;
            if (user_salt != null)
            {
                string user_hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(password: user.Password!, salt: user_salt, prf: KeyDerivationPrf.HMACSHA256, iterationCount: 100000, numBytesRequested: 256 / 8));
                if (user_hashed == user_creds.PasswordHash)
                {
                    return user_role;
                }
                else
                {
                    return "_";
                }
            }
            else
            {
                return "_";
            }
        }

        public UserModel GetEmployeeByEmail(string Email)
        {
            return _authRepository.GetEmployeeByEmail(Email);
        }

        public List<UserModel> GetEmployeesByManager(string Email)
        {
            return _authRepository.GetEmployeesByManager(Email);
        }

        public List<UserModel> GetEmployeesForAdmin(string Email)
        {
            return _authRepository.GetEmployeesForAdmin(Email);
        }

        public UserModel EditEmployee(string Email)
        {
            return _authRepository.EditEmployee(Email);
        }

        public bool UpdateEmployee(UserModel user)
        {
            return _authRepository.UpdateEmployee(user);
        }
    }
}
