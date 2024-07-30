using Admin.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.IRepositories
{
    public interface IAuthRepository
    {
        public bool SignUp(UserModel user);
        public UserModel GetCreds(string Email);
        
        public List<string> GetManagers();

        public List<UserModel> GetEmployeesByManager(string Email);

        public List<UserModel> GetEmployeesForAdmin(string Email);

        public UserModel GetEmployeeByEmail(string Email);

        public UserModel EditEmployee(string Email);

        public bool UpdateEmployee(UserModel user);
    }
}
