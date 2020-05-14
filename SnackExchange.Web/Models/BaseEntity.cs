using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnackExchange.Web.Models
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
