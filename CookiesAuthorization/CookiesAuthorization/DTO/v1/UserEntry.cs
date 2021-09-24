using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookiesAuthorization.DTO.v1
{
    public class UserEntry
    {
        public Guid UserID { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public string Salt { get; set; }
        public byte[] PasswordHash { get; set; }

    }
}
