using CookiesAuthorization.Contracts.v1.Requests;
using CookiesAuthorization.Contracts.v1.Responses;
using CookiesAuthorization.Domain;
using CookiesAuthorization.DTO;
using System.Collections.Generic;
using System.Linq;

namespace CookiesAuthorization.Services
{
    public interface IUserMapper
    {
        public User DomainFromDTO(UserEntry userEntry);
        public UserEntry DTOFromDomain(User user);

        public UserDataResponse ResponseFromDomain(User user);

        public GetUsersQuery DomainFromRequest(GetUsersDataRequest getUsersRequest);
        public UsersDataResponse ResponseFromDomain(List<User> users);
    }

    public class UserMapper : IUserMapper
    {
        public User DomainFromDTO(UserEntry userEntry)
        {
            if (userEntry is null)
                return null;

            var user = new User
            {
                UserID = userEntry.UserID,
                Username = userEntry.Username,
                Role = userEntry.Role,
                PasswordHash = userEntry.PasswordHash,
                Salt = userEntry.Salt
            };

            return user;
        }

        public GetUsersQuery DomainFromRequest(GetUsersDataRequest getUsersRequest)
        {
            if (getUsersRequest is null)
                return null;

            var usersQuery = new GetUsersQuery
            {
                Count = getUsersRequest.Count,
                Offset = getUsersRequest.Offset
            };

            return usersQuery;
        }

        public UserEntry DTOFromDomain(User user)
        {
            if (user is null)
                return null;

            var userEntry = new UserEntry
            {
                UserID = user.UserID,
                Username = user.Username,
                Role = user.Role,
                PasswordHash = user.PasswordHash,
                Salt = user.Salt
            };

            return userEntry;
        }

        public UserDataResponse ResponseFromDomain(User user)
        {
            if (user is null)
                return null;

            var response = new UserDataResponse
            {
                UserName = user.Username,
                Role = user.Role
            };

            return response;
        }

        public UsersDataResponse ResponseFromDomain(List<User> users)
        {
            if (users is null)
                return null;

            var dataResponses = users.Select(x => ResponseFromDomain(x)).ToList();
            var response = new UsersDataResponse
            {
                UserDataResponses = dataResponses
            };

            return response;
        }
    }
}
