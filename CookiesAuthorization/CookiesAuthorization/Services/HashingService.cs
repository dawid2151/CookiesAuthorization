using CookiesAuthorization.DTO.v1;
using System.Security.Cryptography;
using System.Text;

namespace CookiesAuthorization.Services
{
    public interface IHashingService
    {
        public byte[] HashFromString(string data);
        public string SaltFromUserEntry(UserEntry userEntry);
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

        public string SaltFromUserEntry(UserEntry userEntry)
        {
            string salt = userEntry.UserID.ToString();
            return salt;
        }
    }
}
