using Admin.Core.IRepositories;
using Dapper;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Admin.Core.Models;
using System.Security.Cryptography.X509Certificates;
using Admin.Core.Utilities;
using System.Reflection.Metadata;

namespace Admin.Resources
{
    public class AuthRepository : IAuthRepository
    {
        public string connectionString = "Server=13.127.44.211;Database=Express990_TraningDB;User Id=Express_dev_user;Password=Dev@2024;";
        public bool SignUp(UserModel user)
        {           
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    connection.Execute(SQLConstants.sign_up_query, new { NAME = user.Name, EMAIL = user.Email, PASSWORDHASH = user.PasswordHash, SALT = user.Salt, PHONE = user.Phone, ROLEID = user.RoleId, MANAGER = user.Manager });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    return false;
                }
            }
            return true;
        }

        public List<string> GetManagers()
        {
            List<string> result = new List<string>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    result = connection.Query<string>(SQLConstants.get_managers_query).ToList();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
            return result;
        }

        public UserModel GetCreds(string Email)
        {
            UserModel user = new UserModel();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    var creds = connection.QuerySingle(SQLConstants.get_creds_query, new { EMAIL = Email });
                    user.Salt = creds.SALT;
                    user.PasswordHash = creds.PASSWORDHASH;
                    user.RoleId = Convert.ToString(creds.ROLEID);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
                return user;
            }
        }

        public List<UserModel> GetEmployeesByManager(string Email)
        {
            List<UserModel> users = new List<UserModel>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();    
                    UserModel user = connection.QuerySingle<UserModel>(SQLConstants.get_employees_by_email_query, new { EMAIL = Email });
                    users.Add(user);
                    var details = connection.Query<UserModel>(SQLConstants.get_employees_by_manager_query, new { EMAIL = Email }).ToList();
                    foreach (var detail in details)
                    {
                        users.Add(detail);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
            return users;
        }

        public List<UserModel> GetEmployeesForAdmin(string Email)
        {
            List<UserModel> users = new List<UserModel>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    UserModel user = connection.QuerySingle<UserModel>(SQLConstants.get_employees_for_admin_query, new { EMAIL = Email });
                    users.Add(user);
                    var details = connection.Query<UserModel>(SQLConstants.get_employees_for_admin_query);
                    foreach (var detail in details)
                    {
                        users.Add(detail);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
            return users;
        }

        public UserModel GetEmployeeByEmail(string Email)
        {
            UserModel user = new UserModel();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    user = connection.QuerySingle<UserModel>(SQLConstants.get_employees_by_email_query, new { EMAIL = Email });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
            return user;
        }

    }
}
