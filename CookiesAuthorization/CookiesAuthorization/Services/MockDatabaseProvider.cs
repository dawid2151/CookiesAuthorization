using CookiesAuthorization.DTO.v1;
using CookiesAuthorization.Models.v1;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace CookiesAuthorization.Services
{
    public interface IDatabaseProvider
    {
        public List<UserEntry> UserEntries { get; }
    }

    public class MockDatabaseProvider : IDatabaseProvider
    {
        private IHashingService _hashingService;
        public List<UserEntry> UserEntries { get; set; }

        public MockDatabaseProvider(IHashingService hashingService)
        {
            _hashingService = hashingService;

            UserEntries = new List<UserEntry>()
            {
                new UserEntry{
                    UserID = Guid.NewGuid(),
                    Username = "TestUser",
                    Salt = "TestSalt",
                    PasswordHash = _hashingService.HashFromString("TestPassword" + "TestSalt"),
                    Role = "User"
                },
                new UserEntry{
                    UserID = Guid.NewGuid(),
                    Username = "TestAdmin",
                    Salt = "TestAdminSalt",
                    PasswordHash = _hashingService.HashFromString("TestAdminPassword" + "TestAdminSalt"),
                    Role = "Administrator"
                }
            };
        }

    }
}
