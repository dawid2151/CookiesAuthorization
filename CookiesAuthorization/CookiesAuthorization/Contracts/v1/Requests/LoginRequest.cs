using System.ComponentModel.DataAnnotations;


namespace CookiesAuthorization.Contracts.v1.Requests
{
    public class LoginRequest
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
