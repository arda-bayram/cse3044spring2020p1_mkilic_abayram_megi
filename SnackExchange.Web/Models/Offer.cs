using SnackExchange.Web.Models.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SnackExchange.Web.Models
{
    public class Offer : BaseEntity
    {
        public Offer()
        {
            Products = new List<Product>();
        }
        [ForeignKey("Exchange")]
        public Guid ExchangeId { get; set; }
        public virtual Exchange Exchange { get; set; }

        [ForeignKey("Offerer")]
        public string OffererId { get; set; }
        public virtual AppUser Offerer {get;set;}

        public string OfferNotes { get; set; }
        public string PhotoUrl { get; set; }

        public OfferStatus? Status { get; set; } = OfferStatus.None;
        public virtual List<Product> Products { get; set; }
    }
    public enum OfferStatus
    {
        None = 0,
        Waiting = 1,
        Accepted = 2,
        Rejected = 3
    }
}
