using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using CookiesAuthorization.Contracts.v1.Requests;
using CookiesAuthorization.Contracts.v1.Responses;
using CookiesAuthorization.ExtensionMethods;
using CookiesAuthorization.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CookiesAuthorization.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IDatabaseProvider _databaseProvider;

        public UsersController(IDatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm]LoginRequest loginRequest)
        {
            var requestedUser = _databaseProvider.UserEntries.SingleOrDefault(user => user.Username == loginRequest.Username);
            if (requestedUser is null)
                return BadRequest("User does not exist.");
            var hashString = loginRequest.Password + requestedUser.Salt;
            var calculatedHash = MockDatabaseProvider.GetHashFromString(hashString);
            if (!calculatedHash.Compare(requestedUser.PasswordHash))
                return BadRequest("Password is not correct");

            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, requestedUser.Username),
                new Claim(ClaimTypes.Role, requestedUser.Role)
            };
            var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimPrincipal = new ClaimsPrincipal(claimIdentity);


            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                claimPrincipal,
                new AuthenticationProperties
            {
                IsPersistent = true,
                IssuedUtc = DateTime.UtcNow,
                AllowRefresh = true,
                //Use datetime provider for testability
                ExpiresUtc = DateTime.UtcNow.AddDays(7)
            }) ;

            var userResponse = new UserDataResponse { UserName = requestedUser.Username, Role = requestedUser.Role };

            return Ok(userResponse);
        }

        [Authorize]
        [HttpGet("getUserData")]
        public IActionResult GetUserData()
        {
            var username = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if(username is null)
            {
                return Conflict("User does not exist, log in to access resource");
            }
            var user = _databaseProvider.UserEntries.Single(x => x.Username == username.Value);
            if(user is null)
            {
                return Conflict("Authenticated user does not match a user in database.");
            }
            var response = new UserDataResponse { UserName = user.Username, Role = user.Role };
            return Ok(response);
        }
    }
}