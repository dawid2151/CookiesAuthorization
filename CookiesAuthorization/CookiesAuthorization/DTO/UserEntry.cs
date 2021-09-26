using System;

namespace CookiesAuthorization.DTO
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
