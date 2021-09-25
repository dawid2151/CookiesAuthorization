using System.ComponentModel.DataAnnotations;

namespace CookiesAuthorization.Contracts.v1.Requests
{
    public class GetUsersDataRequest
    {
        [Required]
        public int Count { get; set; }
        public int Offset { get; set; }
    }
}
