using CookiesAuthorization.DTO.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookiesAuthorization.Services
{
    public interface IUsersService
    {
        public UserEntry GetUserByUserName(string username);

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
            var user = _databaseProvider.UserEntries.Single(x => x.Username == username);
            return user;
        }
    }
}
