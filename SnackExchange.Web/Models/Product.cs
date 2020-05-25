using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SnackExchange.Web.Models
{
    public class Product : BaseEntity
    {
        public Product()
        {

        }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public virtual Country OriginCountry { get; set; }
        public virtual Exchange Exchange { get; set; }
        [ForeignKey("Offer")]
        public Guid? OfferId { get; set; }
        public virtual Offer Offer { get; set; }
    }
}
