using SnackExchange.Web.Models.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SnackExchange.Web.Models
{
    public class Address : BaseEntity
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public string PlusCode { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual AppUser User { get; set; }

        public virtual Country Country { get; set; }
    }
}
