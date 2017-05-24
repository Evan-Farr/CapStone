using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SyncMe.Models
{
    public class EventInvitation
    {
        [Key]
        public int Id { get; set; }
        [Required, DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [Required]
        public string Status { get; set; }

        public virtual Profile Sender { get; set; }
        public virtual Member Receiver { get; set; }
        public virtual Event Event { get; set; }
    }
}