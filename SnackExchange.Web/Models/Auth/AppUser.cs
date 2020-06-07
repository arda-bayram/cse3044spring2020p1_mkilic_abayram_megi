using Microsoft.AspNetCore.Identity;
using SnackExchange.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
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
        public virtual Country Country { get; set; }
        public virtual List<Post> Posts { get; set; }
        public virtual List<Address> Addresses { get; set; }
        public virtual List<ExchangeUserModel> Exchanges { get; set; }
        public bool IsModerator { get; set; }

        [EnumDataType(typeof(UserStatus))]
        public UserStatus UserStatus { get; set; }
    }
    public class ExchangeUserModel : BaseEntity
    {
        public UserExchangeRole? UserExchangeRole { get; set; }

        public virtual Exchange Exchange { get; set; }

        public virtual AppUser AppUser { get; set; }

    }

    public enum UserExchangeRole
    {
        Unknown = 0,
        Sender = 1,
        Receiver = 2,
        Moderator = 3
    }

    public enum UserStatus
    {
        Inactive = 0,
        Active = 1,
        Banned = 2
    }

}

