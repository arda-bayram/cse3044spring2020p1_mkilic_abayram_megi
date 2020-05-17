using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnackExchange.Web.Models
{
    //enum ExchangeStatus
    //{
    //    Active,
    //    Inactive,
    //    OnProcess,
    //    Reported
    //}

    public class Exchange : BaseEntity
    {
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public int ModeratorId { get; set; }
        public string ModeratorNotes { get; set; }
        //public ExchangeStatus Status { get; set; }
        public string ExchangeNotes { get; set; }
        public string PhotoUrl { get; set; }
        public string TrackingNumber { get; set; }
    }
}
