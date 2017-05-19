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
        public string title { get; set; }
        public string streetAddress { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        [StringLength(5)]
        public string zipCode { get; set; }
        [Required, DataType(DataType.Date)]
        public DateTime start { get; set; }
        [Required, DataType(DataType.Date)]
        public DateTime end { get; set; }
        [DataType(DataType.Time)]
        public DateTime startTime { get; set; }
        [DataType(DataType.Time)]
        public DateTime endTime { get; set; }
        [DataType(DataType.MultilineText)]
        public string details { get; set; }
        [Required]
        public bool isPrivate { get; set; }
        public bool active { get { if (end < DateTime.Today) { return false; } else { return true; } } }

    }
}