using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SyncMe.Models;
using Microsoft.AspNet.Identity;

namespace SyncMe.Controllers
{
    public class MembersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Members
        public ActionResult Index()
        {
            return View(db.Members.ToList());
        }

        // GET: Members/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Member member = db.Members.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        public ActionResult PrivateDetails()
        {
            var current = User.Identity.GetUserId();
            var member = db.Members.Where(m => m.UserId.Id == current).Select(s => s).FirstOrDefault();
            return View(member);
        }

        //public ActionResult ContactRequest(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    var profile = db.Profiles.Where(p => p.Member.Id == id).Select(s => s).FirstOrDefault();
        //    return View(profile);
        //}

        // GET: Members/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Members/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,Age,StreetAddress,City,State,ZipCode,Phone,Email")] Member member)
        {
            if (ModelState.IsValid)
            {
                var holder = User.Identity.GetUserId();
                var same = db.Users.Where(s => s.Id == holder).FirstOrDefault();
                member.UserId = same;
                db.Members.Add(member);
                db.SaveChanges();
                return RedirectToAction("Create", "Profiles");
            }
            return View(member);
        }

        // GET: Members/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Member member = db.Members.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        // POST: Members/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,Age,StreetAddress,City,State,ZipCode,Phone,Email")] Member member)
        {
            if (ModelState.IsValid)
            {
                db.Entry(member).State = EntityState.Modified;
                db.SaveChanges();
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index", "Users");
                }
                return RedirectToAction("Index", "Manage");
            }
            return View(member);
        }

        // GET: Members/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Member member = db.Members.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        // POST: Members/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Member member = db.Members.Find(id);
            db.Members.Remove(member);
            db.SaveChanges();
            return RedirectToAction("Index", "Users");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult ViewCalendar()
        {
            try
            {
                var current = User.Identity.GetUserId();
                var member = db.Members.Where(m => m.UserId.Id == current).Select(s => s).FirstOrDefault();
                var events = new List<Event>();
                foreach (var item in member.Events)
                {
                    events.Add(item);
                }
                return View(events);
            }catch
            {
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index", "Users");
                }else
                {
                    return RedirectToAction("Register", "Account");
                }
            }
        }

        public ActionResult ViewContacts()
        {
            var current = User.Identity.GetUserId();
            var member = db.Members.Where(m => m.UserId.Id == current).Select(s => s).FirstOrDefault();
            //var contacts = new List<Contact>();
            //foreach(var contact in member.Contacts)
            //{
            //    contacts.Add(contact);
            //}
            var contacts = member.Contacts.OrderBy(a => a.LastName).ToList();
            return View(contacts);
        }

        public ActionResult SendContactRequest(int? id)
        {
            var current = User.Identity.GetUserId();
            var member = db.Members.Where(m => m.UserId.Id == current).Select(s => s).FirstOrDefault();
            var sender = db.Profiles.Where(b => b.Member.Id == member.Id).Select(q => q).FirstOrDefault();
            var receiverProfile = db.Profiles.Where(p => p.Id == id).Select(a => a).FirstOrDefault();
            var receiver = db.Members.Where(w => w.Id == receiverProfile.Member.Id).Select(t => t).FirstOrDefault();
            ContactRequest contactRequest = new ContactRequest();
            contactRequest.Sender = sender;
            contactRequest.Receiver = receiver;
            contactRequest.Status = "Pending";
            contactRequest.DateSent = DateTime.Today;
            receiver.ContactRequests.Add(contactRequest);
            db.ContactRequests.Add(contactRequest);
            db.SaveChanges();
            TempData["Message"] = "**Contact request successfully sent!";
            return RedirectToAction("Search", "Profiles");
        }

        public ActionResult ViewContactRequests()
        {
            var user = User.Identity.GetUserId();
            var member = db.Members.Where(u => u.UserId.Id == user).Select(s => s).FirstOrDefault();
            var requests = new List<Profile>();
            foreach(var request in member.ContactRequests)
            {
                var profile = db.Profiles.Where(p => p.Id == request.Sender.Id).Select(a => a).FirstOrDefault();
                requests.Add(profile);
            }
            return View(requests.OrderBy(t => t.LastName));
        }

        public ActionResult AcceptContactRequest(int? id)
        {
            var user = User.Identity.GetUserId();
            var member = db.Members.Where(u => u.UserId.Id == user).Select(s => s).FirstOrDefault();
            var senderProfile = db.Profiles.Where(n => n.Id == id).Select(o => o).FirstOrDefault();
            var sender = db.Members.Where(r => r.Id == senderProfile.Member.Id).Select(k => k).FirstOrDefault();
            var receiverProfile = db.Profiles.Where(l => l.Member.Id == member.Id).Select(i => i).FirstOrDefault();
            var contactRequest = db.ContactRequests.Where(c => c.Sender.Id == sender.Id && c.Receiver.Id == member.Id).Select(a => a).FirstOrDefault();
            contactRequest.Status = "Approved";
            Contact contact = new Contact();
            contact.Id = senderProfile.Id;
            contact.ProfilePictureId = senderProfile.ProfilePictureId;
            contact.FirstName = senderProfile.FirstName;
            contact.LastName = senderProfile.LastName;
            contact.Age = senderProfile.Age;
            contact.State = senderProfile.State;
            contact.CompanyName = senderProfile.CompanyName;
            contact.SchoolName = senderProfile.SchoolName;
            contact.Phone = senderProfile.Phone;
            contact.Email = senderProfile.Email;
            Contact contact2 = new Contact();
            contact2.Id = receiverProfile.Id;
            contact2.ProfilePictureId = receiverProfile.ProfilePictureId;
            contact2.FirstName = receiverProfile.FirstName;
            contact2.LastName = receiverProfile.LastName;
            contact2.Age = receiverProfile.Age;
            contact2.State = receiverProfile.State;
            contact2.CompanyName = receiverProfile.CompanyName;
            contact2.SchoolName = receiverProfile.SchoolName;
            contact2.Phone = receiverProfile.Phone;
            contact2.Email = receiverProfile.Email;
            db.Contacts.Add(contact);
            db.Contacts.Add(contact2);
            member.Contacts.Add(contact);
            sender.Contacts.Add(contact2);
            member.ContactRequests.Remove(contactRequest);
            db.SaveChanges();
            TempData["Message"] = "**You have a new contact!";
            return RedirectToAction("ViewContactRequests");
        }

        public ActionResult DenyContactRequest(int? id)
        {
            var user = User.Identity.GetUserId();
            var member = db.Members.Where(u => u.UserId.Id == user).Select(s => s).FirstOrDefault();
            var senderProfile = db.Profiles.Where(n => n.Id == id).Select(o => o).FirstOrDefault();
            var sender = db.Members.Where(r => r.Id == senderProfile.Member.Id).Select(k => k).FirstOrDefault();
            var contactRequest = db.ContactRequests.Where(c => c.Sender.Id == sender.Id && c.Receiver.Id == member.Id).Select(a => a).FirstOrDefault();
            contactRequest.Status = "Denied";
            member.ContactRequests.Remove(contactRequest);
            db.SaveChanges();
            TempData["Message"] = "**Request was removed from your pending contact requests.";
            return RedirectToAction("ViewContactRequests");
        }

        public ActionResult RemoveContact(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = db.Contacts.Find(id);
            if (contact == null)
            {
                TempData["ErrorMessage"] = "**A problem occurred while removing contact. Please try again later.";
                return RedirectToAction("ViewContacts");
            }
            var current = User.Identity.GetUserId();
            var member = db.Members.Where(m => m.UserId.Id == current).Select(s => s).FirstOrDefault();//member
            var profile = db.Profiles.Where(b => b.Id == contact.Id).Select(x => x).FirstOrDefault(); //contacts profile
            var member2 = db.Members.Where(t => t.Id == profile.Member.Id).Select(o => o).FirstOrDefault(); //contact
            var profile2 = db.Profiles.Where(z => z.Member.Id == member.Id).Select(w => w).FirstOrDefault(); //members profile
            var contact2 = db.Contacts.Where(a => a.Id == profile2.Id).Select(y => y).FirstOrDefault(); //members contact
            member.Contacts.Remove(contact);
            member2.Contacts.Remove(contact2);
            db.Contacts.Remove(contact);
            db.Contacts.Remove(contact2);
            db.SaveChanges();
            TempData["Message"] = "**SyncMe member successfully removed from your contacts.";
            return RedirectToAction("ViewContacts");
        }
    }
}
