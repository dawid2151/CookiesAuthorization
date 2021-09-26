using CookiesAuthorization.DTO;
using System;
using System.Collections.Generic;

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
