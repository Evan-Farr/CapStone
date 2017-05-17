using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SyncMe.Models
{
    public class Calendar
    {
        public Calendar()
        {
            Events = new List<Event>();
        }

        [Key]
        public int Id { get; set; }
        

        public virtual ICollection<Event> Events { get; set; }
    }
}