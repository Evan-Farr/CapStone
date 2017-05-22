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

        public ActionResult ContactRequest(int? id)
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
            var profile = db.Profiles.Where(p => p.Member.Id == member.Id).Select(s => s).FirstOrDefault();
            return View(profile);
        }

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
            return View(member.Contacts.ToList());
        }

        public ActionResult SendContactRequest(int id)
        {
            var current = User.Identity.GetUserId();
            var sender = db.Members.Where(m => m.UserId.Id == current).Select(s => s).FirstOrDefault();
            var profile = db.Profiles.Where(p => p.Id == id).Select(a => a).FirstOrDefault();
            var receiver = db.Members.Where(w => w.Id == profile.Member.Id).Select(t => t).FirstOrDefault();
            ContactRequest contactRequest = new ContactRequest();
            contactRequest.Sender = sender;
            contactRequest.Reciever = receiver;
            contactRequest.Status = "Pending";
            contactRequest.DateSent = DateTime.Today;
            receiver.ContactRequests.Add(contactRequest);
            db.ContactRequests.Add(contactRequest);
            TempData["Message"] = "**Contact request successfully sent!";
            return RedirectToAction("ViewContacts");
        }

        public ActionResult ViewContactRequests()
        {
            var user = User.Identity.GetUserId();
            var requests = db.Members.Where(u => u.UserId.Id == user).Select(s => s.ContactRequests).ToList();
            return View(requests);
        }

        public ActionResult AcceptContactRequest(int? id)
        {
            var user = User.Identity.GetUserId();
            var member = db.Members.Where(u => u.UserId.Id == user).Select(s => s).FirstOrDefault();
            var contactRequest = db.ContactRequests.Where(a => a.Id == id).Select(p => p).FirstOrDefault();
            var profile = db.Profiles.Where(p => p.Member.Id == contactRequest.Sender.Id).Select(k => k).FirstOrDefault();
            var profile2 = db.Profiles.Where(l => l.Member.Id == member.Id).Select(i => i).FirstOrDefault();
            contactRequest.Status = "Approved";
            member.Contacts.Add(profile);
            contactRequest.Sender.Contacts.Add(profile2);
            member.ContactRequests.Remove(contactRequest);
            db.ContactRequests.Remove(contactRequest);
            db.SaveChanges();
            TempData["Message"] = "**You have a new contact!";
            return RedirectToAction("ViewContactRequests");
        }

        public ActionResult DenyContactRequest(int? id)
        {
            var user = User.Identity.GetUserId();
            var member = db.Members.Where(u => u.UserId.Id == user).Select(s => s).FirstOrDefault();
            var contactRequest = db.ContactRequests.Where(a => a.Id == contactRequestId).Select(p => p).FirstOrDefault();
            member.ContactRequests.Remove(contactRequest);
            contactRequest.Status = "Denied";
            db.SaveChanges();
            TempData["Message"] = "**Contact request removed from your pending contact requests.";
            return RedirectToAction("ViewContactRequests");
        }

        public ActionResult RemoveContact(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = db.Profiles.Find(id);
            if (profile == null)
            {
                TempData["ErrorMessage"] = "**A problem occurred while removing contact. Please try again later.";
                return RedirectToAction("ViewContacts");
            }
            var current = User.Identity.GetUserId();
            var member = db.Members.Where(m => m.UserId.Id == current).Select(s => s).FirstOrDefault();
            member.Contacts.Remove(profile);
            TempData["Message"] = "**SyncMe member successfully removed from your contacts.";
            return RedirectToAction("ViewContacts");
        }
    }
}
