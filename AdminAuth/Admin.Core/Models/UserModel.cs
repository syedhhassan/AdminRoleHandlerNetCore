using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Models
{
    public class UserModel
    {
        public Guid? UserID { get; set; }

        public string? Name { get; set; }

        public string? Phone {  get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }
        
        public string? PasswordHash { get; set; }

        public byte[]? Salt { get; set; }

        public string? Role { get; set; }

        public string? Manager { get; set; }

    }
}
