using System;

namespace CookiesAuthorization.Domain
{
    public class User
    {
        public Guid UserID { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public byte[] Salt { get; set; }
        public byte[] PasswordHash { get; set; }
    }
}
