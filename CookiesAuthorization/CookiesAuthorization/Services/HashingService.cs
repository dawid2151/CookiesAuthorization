using CookiesAuthorization.Domain;
using CookiesAuthorization.DTO;
using System.Security.Cryptography;
using System.Text;

namespace CookiesAuthorization.Services
{
    public interface IHashingService
    {
        public byte[] HashFromString(string data);
        public string SaltFromUser(User user);
    }
    public class SHA256HashingService : IHashingService
    {

        public byte[] HashFromString(string data)
        {
            using (SHA256 sha = SHA256.Create())
            {
                return sha.ComputeHash(Encoding.UTF8.GetBytes(data));
            }
        }

        public string SaltFromUser(User user)
        {
            string salt = user.UserID.ToString();
            return salt;
        }
    }
}
