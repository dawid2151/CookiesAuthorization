using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CookiesAuthorization.Contracts.v1;
using CookiesAuthorization.Contracts.v1.Requests;
using CookiesAuthorization.Contracts.v1.Responses;
using CookiesAuthorization.DTO.v1;
using CookiesAuthorization.ExtensionMethods;
using CookiesAuthorization.Models.v1;
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
    public class AuthController : ControllerBase
    {
        private IUsersService _usersService;
        private IHashingService _hashingService;
     
        public AuthController(IUsersService usersService, IHashingService hashingService)
        {
            _usersService = usersService;
            _hashingService = hashingService;
        }
        [HttpPost(Endpoints.Auth.Register)]
        public async Task<IActionResult> Register([FromForm]RegisterRequest registerRequest)
        {
            if (registerRequest is null)
                return BadRequest("Required infromations were not provided.");

            var potentialUser = _usersService.GetUserByUserName(registerRequest.Username);
            if (potentialUser != null)
                return Conflict("User already exists.");

            var userDTO = new UserEntry
            {
                UserID = Guid.NewGuid(),
                Username = registerRequest.Username,
                Role = Roles.User
            };
            string salt = _hashingService.SaltFromUserEntry(userDTO);
            userDTO.Salt = salt;
            userDTO.PasswordHash = _hashingService.HashFromString(registerRequest.Password + salt);

            bool registered = _usersService.AddUserEntry(userDTO);
            if (!registered)
                return Conflict("Can't register provided user.");

            var userFromRequest = new User
            {
                UserID = userDTO.UserID,
                Username = userDTO.Username,
                Role = userDTO.Role
            };

            var claimPrincipal = ClaimPrincipalFromUser(userFromRequest);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                claimPrincipal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    IssuedUtc = DateTime.UtcNow,
                    AllowRefresh = true,
                    //Use datetime provider for testability
                    ExpiresUtc = DateTime.UtcNow.AddDays(7)
                });

            var userResponse = new UserDataResponse
            {
                UserName = userDTO.Username,
                Role = userDTO.Role
            };

            return Accepted(userResponse);

        }

        [HttpPost(Endpoints.Auth.Login)]
        public async Task<IActionResult> Login([FromForm]LoginRequest loginRequest)
        {
            var requestedUser = _usersService.GetUserByUserName(loginRequest.Username);
            if (requestedUser is null)
                return BadRequest("User does not exist.");

            var hashString = loginRequest.Password + requestedUser.Salt;
            var calculatedHash = _hashingService.HashFromString(hashString);
            if (!calculatedHash.Compare(requestedUser.PasswordHash))
                return BadRequest("Password is not correct");

            var userFromRequest = new User
            {
                UserID = requestedUser.UserID,
                Username = requestedUser.Username,
                Role = requestedUser.Role
            };

            var claimPrincipal = ClaimPrincipalFromUser(userFromRequest);

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
        [HttpGet(Endpoints.Auth.Logout)]
        public async Task<IActionResult> Logout()
        {
            var userName = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            var user = _usersService.GetUserByUserName(userName.Value);
            if(user is null)
                return Conflict("Authenticated user does not match a user in database.");

            await HttpContext.SignOutAsync();
            return Ok();
        }


        private ClaimsPrincipal ClaimPrincipalFromUser(User user)
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimPrincipal = new ClaimsPrincipal(claimIdentity);

            return claimPrincipal;
        }
    }
}