using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CookiesAuthorization.Contracts.v1;
using CookiesAuthorization.Contracts.v1.Requests;
using CookiesAuthorization.Contracts.v1.Responses;
using CookiesAuthorization.DTO;
using CookiesAuthorization.ExtensionMethods;
using CookiesAuthorization.Domain;
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
        private IUserMapper _userMapper;
     
        public AuthController(IUsersService usersService, IHashingService hashingService, IUserMapper userMapper)
        {
            _usersService = usersService;
            _hashingService = hashingService;
            _userMapper = userMapper;
        }

        [HttpPost(Endpoints.Auth.Register)]
        public async Task<IActionResult> Register([FromForm]RegisterRequest registerRequest)
        {
            var potentialUser = _usersService.GetUserByUserName(registerRequest.Username);
            if (potentialUser != null)
                return Conflict("User already exists.");

            var user = CreateNewUser(registerRequest);

            bool registered = _usersService.AddUser(user);
            if (!registered)
                return Conflict("Can't register provided user.");

            var claimPrincipal = ClaimPrincipalFromUser(user);

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

            var userResponse = _userMapper.ResponseFromDomain(user);

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


            var claimPrincipal = ClaimPrincipalFromUser(requestedUser);

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

            var userResponse = _userMapper.ResponseFromDomain(requestedUser);

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

        private User CreateNewUser(RegisterRequest registerRequest)
        {
            var user = new User
            {
                UserID = Guid.NewGuid(),
                Username = registerRequest.Username,
                Role = Roles.User
            };
            string salt = _hashingService.SaltFromUser(user);
            user.Salt = salt;
            user.PasswordHash = _hashingService.HashFromString(registerRequest.Password + salt);

            return user;
        }
    }
}