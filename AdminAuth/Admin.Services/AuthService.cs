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

        public bool SignUp(UserModel user)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(password: user.Password!, salt: salt, prf: KeyDerivationPrf.HMACSHA256, iterationCount: 100000, numBytesRequested: 256 / 8));
            user.Salt = salt;
            user.PasswordHash = hashed;
            return _authRepository.SignUp(user);
        }

        public List<string> GetManagers()
        {
            List<string> result = new List<string>();
            result = _authRepository.GetManagers();
            return result;
        }

        public int SignIn(UserModel user)
        {
            var user_creds = _authRepository.GetCreds(user.Email);
            byte[] user_salt = user_creds.Salt;
            var user_role = user_creds.RoleId;
            if (user_salt != null)
            {
                string user_hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(password: user.Password!, salt: user_salt, prf: KeyDerivationPrf.HMACSHA256, iterationCount: 100000, numBytesRequested: 256 / 8));
                if (user_hashed == user_creds.PasswordHash)
                {
                    return Convert.ToInt32(user_role);
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        public List<UserModel> GetEmployeesByManager(string Email)
        {
            return _authRepository.GetEmployeesByManager(Email);
        }
    }
}
