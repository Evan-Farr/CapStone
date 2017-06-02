using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SyncMe.Models
{
    public class GroupCalendar
    {
        public GroupCalendar()
        {
            Members = new List<Member>();
        }

        [Key]
        public int Id { get; set; }
        [Required, DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public int RequestId { get; set; }
        public int Invited { get; set; }

        public virtual Profile Creator { get; set; }
        public virtual ICollection<Member> Members { get; set; }
    }
}