using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnackExchange.Web.Models.Auth
{
    public class AppUser : IdentityUser
    {

        public AppUser()
        {
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CountryCode { get; set; }
        public virtual List<Post> Posts { get; set; }
    }
}
