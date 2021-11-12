using System.Security.Claims;
using CookiesAuthorization.Contracts.v1;
using CookiesAuthorization.Contracts.v1.Requests;
using CookiesAuthorization.Contracts.v1.Responses;
using CookiesAuthorization.Domain;
using CookiesAuthorization.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CookiesAuthorization.Controllers.v1
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUsersService _usersService;
        private IUserMapper _userMapper;

        public UsersController(IUsersService usersService, IUserMapper userMapper)
        {
            _usersService = usersService;
            _userMapper = userMapper;
        }

        [HttpGet(Endpoints.Users.GetUserData)]
        public IActionResult GetUserData()
        {
            var username = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            var user = _usersService.GetUserByUserName(username.Value);
            if (user is null)
            {
                return Conflict("Authenticated user does not match a user in database.");
            }
            var response = _userMapper.ResponseFromDomain(user);
            return Ok(response);
        }
        
        [Authorize(Roles = Roles.Administrator)]
        [HttpGet(Endpoints.Users.GetUsersData)]
        public IActionResult GetUsersData([FromQuery]GetUsersDataRequest getUsers)
        {
            var getUsersQuery = _userMapper.DomainFromRequest(getUsers);
            var users = _usersService.GetUsersBasedOnQuery(getUsersQuery);
            var response = _userMapper.ResponseFromDomain(users);

            return Ok(response);
        }

        [Authorize(Roles = Roles.Administrator)]
        [HttpGet(Endpoints.Users.DeleteUser)]
        public IActionResult DeleteUser([FromQuery]DeleteUserRequest deleteUser)
        {
            var potentialUser = _usersService.GetUserByUserName(deleteUser.Username);
            if (potentialUser is null)
                return BadRequest("User does not exist.");

            var removed = _usersService.RemoveUser(potentialUser);
            if (!removed)
                return Conflict("Could not remove specified user.");

            var response = _userMapper.ResponseFromDomain(potentialUser);

            return Ok(response);
        }
    }
}