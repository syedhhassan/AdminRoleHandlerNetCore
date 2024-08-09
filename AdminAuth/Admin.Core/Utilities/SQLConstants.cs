using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Utilities
{
    public class SQLConstants
    {
        public static string sign_up_query = "INSERT INTO STS892_USER (NAME, EMAIL, PASSWORDHASH, SALT, PHONE, ROLE, MANAGER) VALUES (@NAME, @EMAIL, @PASSWORDHASH, @SALT, @PHONE, @ROLE, @MANAGER);";

        public static string check_duplicate_query = "SELECT COUNT(*) FROM STS892_USER WHERE EMAIL = @EMAIL;";

        public static string get_roles_query = "SELECT TITLE FROM STS892_USERROLES;";

        public static string get_managers_query = "SELECT NAME FROM STS892_USER WHERE ROLE = 'MANAGER' AND IS_ACTIVE = 1;";

        public static string get_creds_query = "SELECT NAME, SALT, PASSWORDHASH, ROLE FROM STS892_USER WHERE EMAIL = @EMAIL AND IS_ACTIVE = 1;";

        public static string get_employees_by_manager_query = "SELECT NAME, PHONE, EMAIL, ROLE, MANAGER, ADDRESS FROM STS892_USER WHERE IS_ACTIVE = 1 AND MANAGER = (SELECT NAME FROM STS892_USER WHERE EMAIL=@EMAIL);";

        public static string get_employee_by_email_query = "SELECT NAME, PHONE, EMAIL, ROLE, MANAGER, ADDRESS FROM STS892_USER WHERE EMAIL = @EMAIL AND IS_ACTIVE = 1;";

        public static string get_employees_for_admin_query = "SELECT NAME, PHONE, EMAIL, ROLE, MANAGER, ADDRESS FROM STS892_USER WHERE ROLE <> 'Admin' AND IS_ACTIVE = 1 ORDER BY ROLE;";

        public static string update_employee_by_email_query = "UPDATE STS892_USER SET NAME = @NAME, PHONE = @PHONE, LAST_MODIFIED=GETDATE() WHERE EMAIL = @EMAIL;";

        public static string delete_employee_by_email_query = "UPDATE STS892_USER SET IS_ACTIVE=0, LAST_MODIFIED=GETDATE() WHERE EMAIL = @EMAIL AND IS_ACTIVE = 1;";

    }
}
