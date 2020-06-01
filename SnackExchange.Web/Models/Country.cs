using SnackExchange.Web.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnackExchange.Web.Models
{
    public class Country : BaseEntity
    {
        public string Name { get; set; }
        public string Currency { get; set; }
        public string Code { get; set; }
        public virtual List<Address> Addresses { get; set; }
        public virtual List<AppUser> Users { get; set; }
    }
}
