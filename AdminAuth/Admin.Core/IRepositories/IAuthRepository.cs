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

        public bool CheckDuplicate(UserModel user);

        public List<string> GetManagers();

        public UserModel GetCreds(string Email);        

        public List<UserModel> GetEmployeesByManager(string Email);

        public List<UserModel> GetEmployeesForAdmin(string Email);

        public UserModel GetEmployeeByEmail(string Email);

        public UserModel EditEmployee(string Email);

        public bool UpdateEmployee(UserModel user);
    }
}
