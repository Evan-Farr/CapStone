using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SyncMe.Models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        [StringLength(5)]
        public string ZipCode { get; set; }
        [Required, DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [Required, DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        [DataType(DataType.Time)]
        public string StartTime { get; set; }
        [DataType(DataType.Time)]
        public string EndTime { get; set; }
        [DataType(DataType.MultilineText)]
        public string Details { get; set; }
        [Required]
        public bool Private { get; set; }
        public bool Active { get { if (EndDate < DateTime.Today) { return false; } else { return true; } } }

    }
}