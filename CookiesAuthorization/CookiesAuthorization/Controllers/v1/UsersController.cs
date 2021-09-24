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
        public IActionResult Login([FromForm]LoginRequest loginRequest)
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
            var claimIdentity = new ClaimsIdentity(claims);
            var claimPrincipal = new ClaimsPrincipal(claimIdentity);

            HttpContext.SignInAsync(claimPrincipal, new AuthenticationProperties
            {
                AllowRefresh = true,
                //Use datetime provider for testability
                ExpiresUtc = DateTime.Now.AddDays(7)
            });

            var userResponse = new UserDataResponse { UserName = requestedUser.Username, Role = requestedUser.Role };

            return Ok(userResponse);
        }
    }
}