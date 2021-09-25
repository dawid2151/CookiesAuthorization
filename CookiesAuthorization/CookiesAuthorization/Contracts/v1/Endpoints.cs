using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookiesAuthorization.Contracts.v1
{
    public static class Endpoints
    {
        public static class Auth
        {
            public const string Login = "login";
            public const string Logout = "logout";
            public const string Register = "register";
        }
        public static class Users
        {
            public const string GetUserData = "getUserData";
            public const string GetUsersData = "getUsersData";
            public const string DeleteUser = "deleteUser";
            public const string UpdateUserData = "updateUserData";
        }
    }
}
