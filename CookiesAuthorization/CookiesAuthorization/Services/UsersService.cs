using CookiesAuthorization.DTO.v1;
using CookiesAuthorization.Models.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace CookiesAuthorization.Services
{
    public interface IUsersService
    {
        public UserEntry GetUserByUserName(string username);
        public bool AddUserEntry(UserEntry userEntry);
        public bool RemoveUserEntry(UserEntry userEntry);
        public List<UserEntry> GetUsersBasedOnQuery(GetUsersQuery query);


    }

    public class UsersService : IUsersService
    {
        private IDatabaseProvider _databaseProvider;
        public UsersService(IDatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider;
        }

        UserEntry IUsersService.GetUserByUserName(string username)
        {
            var user = _databaseProvider.UserEntries.SingleOrDefault(x => x.Username == username);
            return user;
        }

        bool IUsersService.AddUserEntry(UserEntry userEntry)
        {
            _databaseProvider.UserEntries.Add(userEntry);
            return true;
        }

        public List<UserEntry> GetUsersBasedOnQuery(GetUsersQuery query)
        {
            var users = _databaseProvider.UserEntries.OrderBy(x => x.Username).Skip(query.Offset).Take(query.Count).ToList();
            return users;
        }

        public bool RemoveUserEntry(UserEntry userEntry)
        {
            bool contains = _databaseProvider.UserEntries.Contains(userEntry);
            if (!contains)
                return false;

            var removed = _databaseProvider.UserEntries.Remove(userEntry);

            return removed;
        }
    }
}
