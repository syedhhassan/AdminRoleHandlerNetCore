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

        public List<UserModel> GetEmployeesByManager(string Email);

        public List<string> GetManagers();

        public int SignIn(UserModel user);
    }
}
