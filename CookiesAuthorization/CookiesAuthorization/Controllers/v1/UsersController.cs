using System.Security.Claims;
using CookiesAuthorization.Contracts.v1;
using CookiesAuthorization.Contracts.v1.Requests;
using CookiesAuthorization.Contracts.v1.Responses;
using CookiesAuthorization.Models.v1;
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

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
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
            var response = new UserDataResponse { UserName = user.Username, Role = user.Role };
            return Ok(response);
        }
        
        [Authorize(Roles = Roles.Administrator)]
        [HttpGet(Endpoints.Users.GetUsersData)]
        public IActionResult GetUsersData([FromQuery]GetUsersDataRequest getUsers)
        {
            var getUsersQuery = new GetUsersQuery
            {
                Count = getUsers.Count,
                Offset = getUsers.Offset
            };

            var users = _usersService.GetUsersBasedOnQuery(getUsersQuery);

            return Ok(users);
        }

        [Authorize(Roles = Roles.Administrator)]
        [HttpGet(Endpoints.Users.DeleteUser)]
        public IActionResult DeleteUser([FromQuery]DeleteUserRequest deleteUser)
        {
            var potentialUser = _usersService.GetUserByUserName(deleteUser.Username);
            if (potentialUser is null)
                return BadRequest("User does not exist.");

            var removed = _usersService.RemoveUserEntry(potentialUser);
            if (!removed)
                return Conflict("Could not remove specified user.");

            var response = new UserDataResponse
            {
                UserName = potentialUser.Username,
                Role = potentialUser.Role
            };

            return Ok(response);
        }
    }
}