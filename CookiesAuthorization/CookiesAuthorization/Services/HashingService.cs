using System.Security.Cryptography;
using System.Text;

namespace CookiesAuthorization.Services
{
    public interface IHashingService
    {
        public byte[] HashFromString(string data);
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
    }
}
