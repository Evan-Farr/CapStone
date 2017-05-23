using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SyncMe.Models
{
    public class ContactRequest
    {
        [Key]
        public int Id { get; set; }
        [Required, DataType(DataType.Date)]
        public DateTime DateSent { get; set; }
        [Required]
        public string Status { get; set; }

        public virtual Profile Sender { get; set; }
        public virtual Member Receiver { get; set; }
    }
}