using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SyncMe.Models
{
    public class Member
    {
        public Member()
        {
            Contacts = new List<Profile>();
            ContactRequests = new List<ContactRequest>();
            SyncRequests = new List<SyncRequest>();
            EventInvitations = new List<EventInvitation>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public int? Age { get; set; }
        [Required]
        public string StreetAddress { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required, StringLength(5)]
        public string ZipCode { get; set; }
        [StringLength(10)]
        public string Phone { get; set; }
        public string Email { get; set; }


        public virtual ICollection<Profile> Contacts { get; set; }
        public virtual ICollection<ContactRequest> ContactRequests { get; set; }
        public virtual ICollection<SyncRequest> SyncRequests { get; set; }
        public virtual ICollection<EventInvitation> EventInvitations { get; set; }
        public virtual Profile Profile { get; set; }
        public virtual Calendar Calendar { get; set; }
        public virtual ApplicationUser UserId { get; set; }
    }
}