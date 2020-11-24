using System;
using System.Collections.Generic;
using System.Text;

namespace HYDB.Services.Models
{
    public class UserAccount
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
