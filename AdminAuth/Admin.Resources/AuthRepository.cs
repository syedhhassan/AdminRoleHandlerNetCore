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

        #region Sign Up
        /// <summary>
        /// Sign Up
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool SignUp(UserModel user)
        {           
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    connection.Execute(SQLConstants.sign_up_query, new { NAME = user.Name, EMAIL = user.Email, PASSWORDHASH = user.PasswordHash, SALT = user.Salt, PHONE = user.Phone, ROLE = user.Role, MANAGER = user.Manager });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region Check for duplicate email for sign up
        /// <summary>
        /// Check for duplicate email for sign up
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool CheckDuplicate(UserModel user)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    int count = connection.Execute(SQLConstants.check_duplicate_query, new { EMAIL = user.Email });
                    return count == 0;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region Get Managers for Sign Up dropdown
        /// <summary>
        /// Get Managers for Sign Up dropdown
        /// </summary>
        /// <returns></returns>
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
        #endregion

        #region Getting credentials for signing in
        /// <summary>
        /// Getting credentials for signing in
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
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
                    user.Role = Convert.ToString(creds.ROLE);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
                return user;
            }
        }
        #endregion

        #region Getting employees by their manager
        /// <summary>
        /// Getting employees by their manager
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
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
        #endregion

        #region Getting all employees for admin
        /// <summary>
        /// Getting all employees for admin
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public List<UserModel> GetEmployeesForAdmin(string Email)
        {
            List<UserModel> users = new List<UserModel>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    UserModel user = connection.QuerySingle<UserModel>(SQLConstants.get_employees_by_email_query, new { EMAIL = Email });
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
        #endregion

        #region Getting an employee by Email
        /// <summary>
        /// Getting an employee by Email
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
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
        #endregion

        #region Getting an employee's details for editing
        /// <summary>
        /// Getting an employee's details for editing
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public UserModel EditEmployee(string Email)
            {
            UserModel userModel = new UserModel();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    var user = connection.QuerySingle(SQLConstants.get_employees_by_email_query, new { EMAIL = Email });
                    userModel.Name = user.NAME;
                    userModel.Email = user.EMAIL;
                    userModel.Phone = user.PHONE;
                    userModel.Role = user.ROLE;
                    userModel.Manager = user.MANAGER;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Write("Error executing query: " + ex.Message);
                }
            }
            return userModel;
        }
        #endregion

        #region Updating an employee
        /// <summary>
        /// Updating an employee
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UpdateEmployee(UserModel user)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    connection.Execute(SQLConstants.update_employee_by_email_query, new { NAME = user.Name, EMAIL = user.Email, PHONE = user.Phone, ROLE = user.Role, MANAGER = user.Manager });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    return false;
                }
            }
            return true;
        }
        #endregion

    }
}
