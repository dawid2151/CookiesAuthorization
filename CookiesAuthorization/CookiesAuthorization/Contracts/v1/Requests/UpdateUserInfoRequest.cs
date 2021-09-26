using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookiesAuthorization.Contracts.v1.Requests
{
    public class UpdateUserInfoRequest
    {
        public IFormFile AvatarFile { get; set; } = null;
        public string Description { get; set; } = "Let us know you!";

    }
}
