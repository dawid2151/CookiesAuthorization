using CookiesAuthorization.DTO;
using CookiesAuthorization.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CookiesAuthorization.Services
{
    public interface IUsersService
    {
        public User GetUserByUserName(string username);
        public bool AddUser(User userEntry);
        public bool RemoveUser(User userEntry);
        public List<User> GetUsersBasedOnQuery(GetUsersQuery query);

    }

    public class UsersService : IUsersService
    {
        private IDatabaseProvider _databaseProvider;
        private IUserMapper _userMapper;
        public UsersService(IDatabaseProvider databaseProvider, IUserMapper userMapper)
        {
            _databaseProvider = databaseProvider;
            _userMapper = userMapper;
        }

        User IUsersService.GetUserByUserName(string username)
        {
            var userEntry = _databaseProvider.UserEntries.SingleOrDefault(x => x.Username == username);
            var user = _userMapper.DomainFromDTO(userEntry);
            return user;
        }

        bool IUsersService.AddUser(User user)
        {
            var userEntry = _userMapper.DTOFromDomain(user);
            _databaseProvider.UserEntries.Add(userEntry);
            return true;
        }

        public List<User> GetUsersBasedOnQuery(GetUsersQuery query)
        {
            var userEntries = _databaseProvider.UserEntries.OrderBy(x => x.Username).Skip(query.Offset).Take(query.Count).ToList();
            var users = userEntries.Select(x => _userMapper.DomainFromDTO(x)).ToList();
            return users;
        }

        public bool RemoveUser(User user)
        {
            var userEntry = _userMapper.DTOFromDomain(user);

            bool contains = _databaseProvider.UserEntries.Contains(userEntry);
            if (!contains)
                return false;

            var removed = _databaseProvider.UserEntries.Remove(userEntry);

            return removed;
        }
    }
}
