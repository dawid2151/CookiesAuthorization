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
            byte[] salt1 = _hashingService.SaltFromUser(new Domain.User { UserID = Guid.NewGuid() });
            byte[] salt2 = _hashingService.SaltFromUser(new Domain.User { UserID = Guid.NewGuid() });

            UserEntries = new List<UserEntry>()
            {
                new UserEntry{
                    UserID = Guid.NewGuid(),
                    Username = "TestUser",
                    Salt = salt1,
                    PasswordHash = _hashingService.SaltedHashFromPassword("TestPassword", salt1),
                    Role = "User"
                },
                new UserEntry{
                    UserID = Guid.NewGuid(),
                    Username = "TestAdmin",
                    Salt = salt2,
                    PasswordHash = _hashingService.SaltedHashFromPassword("TestAdminPassword", salt2),
                    Role = "Administrator"
                }
            };
        }

    }
}
