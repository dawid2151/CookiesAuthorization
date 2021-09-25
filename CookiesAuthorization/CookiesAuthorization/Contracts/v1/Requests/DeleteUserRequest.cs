using System.ComponentModel.DataAnnotations;


namespace CookiesAuthorization.Contracts.v1.Requests
{
    public class DeleteUserRequest
    {
        [Required]
        public string Username { get; set; }
    }
}
