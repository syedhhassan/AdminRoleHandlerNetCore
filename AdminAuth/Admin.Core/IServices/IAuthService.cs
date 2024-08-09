using Admin.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.IServices
{
    public interface IAuthService
    {
        public bool SignUp(UserModel user);

        public bool CheckDuplicate(UserModel user);

        public List<List<string>> GetDetails();

        public List<string> GetRoles();

        public List<string> GetManagers();

        public UserModel GetEmployeeByEmail(string Email);

        public List<UserModel> GetEmployeesForAdmin(string Email);

        public List<UserModel> GetEmployeesByManager(string Email);

        public List<string> SignIn(UserModel user);

        public UserModel EditEmployee(string Email);

        public bool UpdateEmployee(UserModel user);

        public bool DeleteEmployee(string Email);

    }
}
