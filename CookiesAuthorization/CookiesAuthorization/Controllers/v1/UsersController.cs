using System.Security.Claims;
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

        [HttpGet("getUserData")]
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
        
        [Authorize(Roles = "Administrator")]
        [HttpGet("getUsersData")]
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
    }
}