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
        public int Code { get; set; }   
    }
}
