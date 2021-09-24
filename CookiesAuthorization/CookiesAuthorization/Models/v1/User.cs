using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookiesAuthorization.Models.v1
{
    public class User
    {
        public Guid UserID { get; set; }
        public string Username { get; set; }
    }
}
