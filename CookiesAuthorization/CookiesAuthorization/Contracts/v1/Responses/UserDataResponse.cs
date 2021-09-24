using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookiesAuthorization.Contracts.v1.Responses
{
    public class UserDataResponse
    {
        public string UserName { get; set; }
        public string Role { get; set; }
    }
}
