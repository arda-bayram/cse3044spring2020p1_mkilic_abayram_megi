using SnackExchange.Web.Models.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SnackExchange.Web.Models
{

    public class Exchange : BaseEntity
    {
        public Exchange()
        {
            Products = new List<Product>();
        }
        public string SenderId { get; set; }
        public string? ReceiverId { get; set; }
        public string? ModeratorId { get; set; }
        public string? ModeratorNotes { get; set; }
        public string ExchangeNotes { get; set; }
        public string PhotoUrl { get; set; }
        public string? TrackingNumber { get; set; }
        public virtual AppUser Sender { get; set; }
        public virtual AppUser Receiver { get; set; }
        public virtual AppUser Moderator { get; set; }
        public ExchangeStatus? Status { get;  set; } = ExchangeStatus.None;
        public virtual List<Product> Products { get; set; }
        public virtual List<Offer> Offers { get; set; }
    }

    public enum ExchangeStatus
    {
        None = 0,
        Created = 1,
        Accepted = 2,
        OnAir = 3,
        Completed = 4
    }
}
