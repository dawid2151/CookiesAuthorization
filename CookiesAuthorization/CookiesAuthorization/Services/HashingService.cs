using CookiesAuthorization.Domain;
using CookiesAuthorization.DTO;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace CookiesAuthorization.Services
{
    public interface IHashingService
    {
        public byte[] SaltedHashFromPassword(string password, byte[] salt);
        public byte[] SaltFromUser(User user);
    }
    
    public class DefaultHashingService : IHashingService
    {
        public byte[] SaltedHashFromPassword(string password, byte[] salt)
        {
            byte[] hash = KeyDerivation.Pbkdf2(
                password,
                salt,
                KeyDerivationPrf.HMACSHA256,
                10000,
                256 / 8);

            return hash;
        }
        public byte[] SaltFromUser(User user)
        {
            byte[] salt = new byte[128 / 8]; //allocate 128bit array
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetNonZeroBytes(salt);
            }
            return salt;
        }
    }

    
    #region Testing only
    [Obsolete("Basic SHA256 was only for testing purposes. It's weak hash and salt is bad - don't use it.")]
    public class SHA256HashingService : IHashingService
    {

        public byte[] SaltedHashFromPassword(string password, byte[] salt)
        {
            byte[] saltPassword = salt.Concat(Encoding.ASCII.GetBytes(password)).ToArray();
            using (SHA256 sha = SHA256.Create())
            {
                return sha.ComputeHash(saltPassword);
            }
        }

        public byte[] SaltFromUser(User user)
        {
            string salt = user.UserID.ToString();
            return Encoding.ASCII.GetBytes(salt);
        }
    }

    #endregion
}
