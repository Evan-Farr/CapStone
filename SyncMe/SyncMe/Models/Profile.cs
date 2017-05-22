using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SyncMe.Models
{
    public class Profile
    {
        [Key]
        public int Id { get; set; }
        public byte[] ProfilePictureId { get; set; }
        public string FirstName { get { return this.Member.FirstName; } }
        public string LastName { get { return this.Member.LastName; } }
        public int? Age { get { return this.Member.Age; } }
        public string State { get { return this.Member.City; } }
        public string CompanyName { get; set; }
        public string SchoolName { get; set; }
        [StringLength(10)]
        public string Phone { get; set; }
        public string Email { get; set; }

        public virtual Member Member { get; set; }
    }
}