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
using SendGrid;
using SendGrid.Helpers.Mail;

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
                var syncRequests = new List<SyncRequest>();
                foreach(var request in member.SyncRequests)
                {
                    if(request.Status == "Pending")
                    {
                        syncRequests.Add(request);
                    }
                }
                foreach (var item in member.Events)
                {
                    events.Add(item);
                }
                if(member.EventInvitations.Count != 0)
                {
                    ViewBag.EventInvitations = member.EventInvitations.Count;
                }
                if (member.ContactRequests.Count != 0)
                {
                    ViewBag.ContactRequests = member.ContactRequests.Count;
                }
                if (member.SyncRequests.Count != 0)
                {
                    ViewBag.SyncRequests = syncRequests.Count;
                }
                if (member.GroupSyncRequests.Count != 0)
                {
                    ViewBag.GroupSyncRequests = member.GroupSyncRequests.Count;
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
            if (member.Contacts.Count == 0)
            {
                TempData["ErrorMessage"] = "**You currently don't have any contacts...";
                return RedirectToAction("ViewCalendar");
            }
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
            if (member.ContactRequests.Count == 0)
            {
                TempData["ErrorMessage"] = "**You currently don't have any pending contact requests.";
                return RedirectToAction("ViewCalendar");
            }
            foreach (var request in member.ContactRequests)
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
            contact.ContactId = senderProfile.Id;
            contact.FriendId = member.Id;
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
            contact2.ContactId = receiverProfile.Id;
            contact2.FriendId = sender.Id;
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
            db.ContactRequests.Remove(contactRequest);
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
            db.ContactRequests.Remove(contactRequest);
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
                TempData["ErrorMessage"] = "**A problem occurred while attempting to remove contact.";
                return RedirectToAction("ViewContacts");
            }
            var otherProfile = db.Profiles.Where(p => p.Id == contact.ContactId).Select(n => n).FirstOrDefault();
            var current = User.Identity.GetUserId();
            var member = db.Members.Where(m => m.UserId.Id == current).Select(s => s).FirstOrDefault();
            var member2 = db.Members.Where(t => t.Id == otherProfile.Member.Id).Select(o => o).FirstOrDefault(); 
            var profile2 = db.Profiles.Where(z => z.Member.Id == member.Id).Select(w => w).FirstOrDefault(); 
            var contact2 = db.Contacts.Where(a => a.ContactId == profile2.Id && a.FriendId == member2.Id).Select(y => y).FirstOrDefault();
            member.Contacts.Remove(contact);
            member2.Contacts.Remove(contact2);
            db.Contacts.Remove(contact);
            db.Contacts.Remove(contact2);
            try
            {
                var syncRequest1 = db.SyncRequests.Where(b => b.Sender.Id == profile2.Id && b.Receiver.Id == member2.Id).Select(v => v).FirstOrDefault();
                var syncRequest2 = db.SyncRequests.Where(g => g.Sender.Id == otherProfile.Id && g.Receiver.Id == member.Id).Select(u => u).FirstOrDefault();
                member.SyncRequests.Remove(syncRequest2);
                member2.SyncRequests.Remove(syncRequest1);
                db.SyncRequests.Remove(syncRequest2);
                db.SyncRequests.Remove(syncRequest1);
            }catch
            {
                
            }finally
            {
                db.SaveChanges();
            }
            TempData["Message"] = "**SyncMe member successfully removed from your contacts.";
            return RedirectToAction("ViewContacts");
        }

        public ActionResult SendEventInvitation(int? eventId, int? id)
        {
            var current = User.Identity.GetUserId();
            var member = db.Members.Where(m => m.UserId.Id == current).Select(s => s).FirstOrDefault();
            var sender = db.Profiles.Where(b => b.Member.Id == member.Id).Select(q => q).FirstOrDefault();
            var receiverProfile = db.Profiles.Where(p => p.Id == id).Select(a => a).FirstOrDefault();
            var receiver = db.Members.Where(w => w.Id == receiverProfile.Member.Id).Select(t => t).FirstOrDefault();
            var @event = db.Events.Where(c => c.Id == eventId).Select(i => i).FirstOrDefault();
            EventInvitation eventInvitation = new EventInvitation();
            eventInvitation.Sender = sender;
            eventInvitation.Receiver = receiver;
            eventInvitation.Status = "Pending";
            eventInvitation.Date = DateTime.Today;
            eventInvitation.Event = @event;
            receiver.EventInvitations.Add(eventInvitation);
            db.EventInvitations.Add(eventInvitation);
            var msg = new SendGridMessage();
            msg.SetFrom(new EmailAddress("evan.c.farr@gmailc.om", "SyncMe"));
            var recipients = new List<EmailAddress>
            {
                new EmailAddress(receiver.Email, receiver.FirstName + " " + receiver.LastName)
            };
            msg.AddTos(recipients);
            msg.SetSubject(sender.FirstName + sender.LastName + " sent you a new event invitation!");
            msg.AddContent(MimeType.Text, "Sign into your SyncMe account to accept or deny " + sender.FirstName + sender.LastName + "'s event invitation.");
            msg.AddContent(MimeType.Html, "<p>Sign into your SyncMe account to accept or deny + sender.FirstName + sender.LastName + 's event invitation.</p>");
            db.SaveChanges();
            TempData["Message"] = "**Event invitation successfully sent!";
            return RedirectToAction("ChooseContacts", new { id = @event.Id});
        }

        public ActionResult ChooseContacts(int? id)
        {
            var current = User.Identity.GetUserId();
            var member = db.Members.Where(m => m.UserId.Id == current).Select(s => s).FirstOrDefault();
            if(member.Contacts.Count == 0)
            {
                TempData["ErrorMessage"] = "**You currently don't have any contacts to send this event to...";
                return RedirectToAction("ViewCalendar");
            }
            var profiles = new List<Profile>();
            foreach(var contact in member.Contacts)
            {
                var profile = db.Profiles.Where(p => p.Id == contact.ContactId).Select(a => a).FirstOrDefault();
                profiles.Add(profile);
            }
            ViewBag.AllEventIvitations = 0;
            if(db.EventInvitations.ToList().Count != 0)
            {
                var allEventInvitations = db.EventInvitations.ToList();
                ViewBag.AllEventInvitations = allEventInvitations;
            }
            var sender = db.Profiles.Where(q => q.Member.Id == member.Id).Select(u => u).FirstOrDefault();
            var @event = db.Events.Where(e => e.Id == id).Select(w => w).FirstOrDefault();
            ViewBag.SenderId = sender.Id;
            ViewBag.EventId = @event.Id;
            ViewBag.counter = 0;
            return View(profiles.OrderBy(o => o.LastName));
        }

        public ActionResult ViewEventInvitations()
        {
            var user = User.Identity.GetUserId();
            var member = db.Members.Where(u => u.UserId.Id == user).Select(s => s).FirstOrDefault();
            var invites = new List<EventInvitation>();
            if (member.EventInvitations.Count == 0)
            {
                TempData["ErrorMessage"] = "**You currently don't have any pending event invitations.";
                return RedirectToAction("ViewCalendar");
            }
            foreach (var invite in member.EventInvitations)
            {
                invites.Add(invite);
            }
            return View(invites.OrderBy(t => t.Date));
        }

        public ActionResult AcceptEventInvitation(int? id)
        {
            var user = User.Identity.GetUserId();
            var member = db.Members.Where(u => u.UserId.Id == user).Select(s => s).FirstOrDefault();
            var eventInvitation = db.EventInvitations.Where(n => n.Id == id).Select(o => o).FirstOrDefault();
            var @event = db.Events.Where(e => e.Id == eventInvitation.Event.Id).Select(y => y).FirstOrDefault();
            eventInvitation.Status = "Approved";
            var newEvent = new Event();
            newEvent.title = @event.title;
            newEvent.streetAddress = @event.streetAddress;
            newEvent.city = @event.city;
            newEvent.state = @event.state;
            newEvent.zipCode = @event.zipCode;
            newEvent.start = @event.start;
            newEvent.end = @event.end;
            newEvent.startTime = @event.startTime;
            newEvent.endTime = @event.endTime;
            newEvent.details = @event.details;
            member.Events.Add(newEvent);
            member.EventInvitations.Remove(eventInvitation);
            db.EventInvitations.Remove(eventInvitation);
            db.SaveChanges();
            TempData["Message"] = "**You have a new event!";
            return RedirectToAction("ViewEventInvitations");
        }

        public ActionResult DenyEventInvitation(int? id)
        {
            var user = User.Identity.GetUserId();
            var member = db.Members.Where(u => u.UserId.Id == user).Select(s => s).FirstOrDefault();
            var eventInvitation = db.EventInvitations.Where(n => n.Id == id).Select(o => o).FirstOrDefault();
            eventInvitation.Status = "Denied";
            member.EventInvitations.Remove(eventInvitation);
            db.EventInvitations.Remove(eventInvitation);
            db.SaveChanges();
            TempData["Message"] = "**Invitation was removed from your pending event invitations.";
            return RedirectToAction("ViewEventInvitations");
        }

        public ActionResult SendSyncRequest(int? id, string reRoute)
        {
            var current = User.Identity.GetUserId();
            var member = db.Members.Where(m => m.UserId.Id == current).Select(s => s).FirstOrDefault();
            var sender = db.Profiles.Where(b => b.Member.Id == member.Id).Select(q => q).FirstOrDefault();
            var receiverProfile = db.Profiles.Where(p => p.Id == id).Select(a => a).FirstOrDefault();
            var receiver = db.Members.Where(w => w.Id == receiverProfile.Member.Id).Select(t => t).FirstOrDefault();
            SyncRequest syncRequest = new SyncRequest();
            syncRequest.Sender = sender;
            syncRequest.Receiver = receiver;
            syncRequest.Status = "Pending";
            syncRequest.Date = DateTime.Today;
            receiver.SyncRequests.Add(syncRequest);
            db.SyncRequests.Add(syncRequest);
            db.SaveChanges();
            if (reRoute == "ChooseSyncContacts")
            {
                TempData["Message"] = "**Sync request successfully sent!";
                return RedirectToAction("ChooseSyncContacts");
            }
            if (reRoute == "ChooseGroupSyncContacts")
            {
                TempData["Message"] = "**Group sync requeset successfully sent!";
                return RedirectToAction("ViewCalendar");
            }
            TempData["ErrorMessage"] = "**An issue occured while attempting to send the sync request.";
            return RedirectToAction("ViewCalendar");
        }

        public ActionResult ChooseSyncContacts()
        {
            var current = User.Identity.GetUserId();
            var member = db.Members.Where(m => m.UserId.Id == current).Select(s => s).FirstOrDefault();
            var memberProfile = db.Profiles.Where(t => t.Member.Id == member.Id).Select(k => k).FirstOrDefault();
            if (member.Contacts.Count == 0)
            {
                TempData["ErrorMessage"] = "**You currently don't have any contacts to send a sync request to...";
                return RedirectToAction("ViewCalendar");
            }
            var profiles = new List<Profile>();
            foreach (var contact in member.Contacts)
            {
                var profile = db.Profiles.Where(p => p.Id == contact.ContactId).Select(a => a).FirstOrDefault();
                profiles.Add(profile);
            }
            ViewBag.AllSyncRequests = new List<SyncRequest>();
            if (db.SyncRequests.ToList().Count != 0)
            {
                foreach(var syncRequest in db.SyncRequests.ToList())
                {
                    ViewBag.AllSyncRequests.Add(syncRequest);
                }
            }
            var sender = db.Profiles.Where(q => q.Member.Id == member.Id).Select(u => u).FirstOrDefault();
            ViewBag.SenderId = sender.Id;
            ViewBag.MemberId = member.Id;
            ViewBag.SenderId2 = memberProfile.Id;
            ViewBag.counter = 0;
            return View(profiles.OrderBy(o => o.LastName));
        }

        public ActionResult SendGroupSyncRequest(string reRoute, List<int> selectedProfiles)
        {
            var current = User.Identity.GetUserId();
            var member = db.Members.Where(m => m.UserId.Id == current).Select(s => s).FirstOrDefault();
            var sender = db.Profiles.Where(b => b.Member.Id == member.Id).Select(q => q).FirstOrDefault();
            var receivers = new List<Member>();
            foreach(var profileId in selectedProfiles)
            {
                if(profileId != 0)
                {
                    var receiverProfile = db.Profiles.Where(p => p.Id == profileId).Select(a => a).FirstOrDefault();
                    var receiver = db.Members.Where(w => w.Id == receiverProfile.Member.Id).Select(t => t).FirstOrDefault();
                    receivers.Add(receiver);
                }
            }
            if (receivers.Count == 0)
            {
                TempData["ErrorMessage"] = "**You did not select any contacts...";
                return RedirectToAction("ChooseGroupSyncContacts");
            }
            if(receivers.Count >= 5)
            {
                TempData["ErrorMessage"] = "**You may only select up to four contacts per group sync request.";
                return RedirectToAction("ChooseGroupSyncContacts");
            }
            GroupSyncRequest syncRequest = new GroupSyncRequest();
            syncRequest.Sender = sender;
            syncRequest.Receivers = receivers;
            syncRequest.Invited = syncRequest.Receivers.Count;
            syncRequest.Status = "Pending";
            syncRequest.Date = DateTime.Today;
            foreach(var person in receivers)
            {
                person.GroupSyncRequests.Add(syncRequest);
            }
            db.GroupSyncRequests.Add(syncRequest);
            db.SaveChanges();
            GroupCalendar groupCalendar = new GroupCalendar();
            groupCalendar.Date = DateTime.Today;
            groupCalendar.RequestId = syncRequest.Id;
            groupCalendar.Invited = syncRequest.Receivers.Count;
            groupCalendar.Creator = sender;
            groupCalendar.Members.Add(member);
            member.GroupCalendars.Add(groupCalendar);
            db.SaveChanges();
            if (reRoute == "ChooseGroupSyncContacts")
            {
                TempData["Message"] = "**Group sync request successfully sent!";
                return RedirectToAction("ViewCalendar");
            }
            TempData["ErrorMessage"] = "**An unknown issue occured while attempting to send the group sync request.";
            return RedirectToAction("ViewCalendar");
        }

        public ActionResult ChooseGroupSyncContacts()
        {
            var current = User.Identity.GetUserId();
            var member = db.Members.Where(m => m.UserId.Id == current).Select(s => s).FirstOrDefault();
            if (member.Contacts.Count == 0)
            {
                TempData["ErrorMessage"] = "**You currently don't have any contacts to send a group sync request to...";
                return RedirectToAction("ViewCalendar");
            }
            var profiles = new List<Profile>();
            foreach (var contact in member.Contacts)
            {
                var profile = db.Profiles.Where(p => p.Id == contact.ContactId).Select(a => a).FirstOrDefault();
                profiles.Add(profile);
            }
            return View(profiles.OrderBy(o => o.LastName));
        }

        public ActionResult ViewSyncRequests()
        {
            var user = User.Identity.GetUserId();
            var member = db.Members.Where(u => u.UserId.Id == user).Select(s => s).FirstOrDefault();
            var requests = new List<SyncRequest>();
            if(member.SyncRequests.Count == 0)
            {
                TempData["ErrorMessage"] = "**You currently don't have any pending sync requests.";
                return RedirectToAction("ViewCalendar");
            }
            foreach (var request in member.SyncRequests)
            {
                if(request.Status != "Approved")
                {
                    requests.Add(request);
                }
            }
            if (requests.Count == 0)
            {
                TempData["ErrorMessage"] = "**You currently don't have any pending sync requests.";
                return RedirectToAction("ViewCalendar");
            }
            return View(requests.OrderBy(t => t.Date));
        }

        public ActionResult ViewGroupSyncRequests()
        {
            var user = User.Identity.GetUserId();
            var member = db.Members.Where(u => u.UserId.Id == user).Select(s => s).FirstOrDefault();
            var requests = new List<GroupSyncRequest>();
            if (member.GroupSyncRequests.Count == 0)
            {
                TempData["ErrorMessage"] = "**You currently don't have any pending group sync requests.";
                return RedirectToAction("ViewCalendar");
            }
            foreach (var request in member.GroupSyncRequests)
            {
                if (request.Status != "Approved")
                {
                    requests.Add(request);
                }
            }
            if (requests.Count == 0)
            {
                TempData["ErrorMessage"] = "**You currently don't have any pending group sync requests.";
                return RedirectToAction("ViewCalendar");
            }
            return View(requests.OrderBy(t => t.Date));
        }

        public ActionResult AcceptSyncRequest(int? id)
        {
            var user = User.Identity.GetUserId();
            var member = db.Members.Where(u => u.UserId.Id == user).Select(s => s).FirstOrDefault();
            var memberProfile = db.Profiles.Where(q => q.Member.Id == member.Id).Select(e => e).FirstOrDefault();
            var syncRequest = db.SyncRequests.Where(n => n.Id == id).Select(o => o).FirstOrDefault();
            var senderProfile = db.Profiles.Where(p => p.Id == syncRequest.Sender.Id).Select(a => a).FirstOrDefault();
            var sender = db.Members.Where(b => b.Id == senderProfile.Member.Id).Select(y => y).FirstOrDefault();
            syncRequest.Status = "Approved";
            var newSyncRequest = new SyncRequest();
            newSyncRequest.Date = syncRequest.Date;
            newSyncRequest.Status = syncRequest.Status;
            newSyncRequest.Receiver = sender;
            newSyncRequest.Sender = memberProfile;
            db.SyncRequests.Add(newSyncRequest);
            sender.SyncRequests.Add(newSyncRequest);
            db.SaveChanges();
            TempData["Message"] = "**You have a new synced calendar!";
            return RedirectToAction("ViewSyncRequests");
        }

        public ActionResult AcceptGroupSyncRequest(int? id)
        {
            var user = User.Identity.GetUserId();
            var member = db.Members.Where(u => u.UserId.Id == user).Select(s => s).FirstOrDefault();
            var memberProfile = db.Profiles.Where(q => q.Member.Id == member.Id).Select(e => e).FirstOrDefault();
            var syncRequest = db.GroupSyncRequests.Where(n => n.Id == id).Select(o => o).FirstOrDefault();
            var senderProfile = db.Profiles.Where(p => p.Id == syncRequest.Sender.Id).Select(a => a).FirstOrDefault();
            var sender = db.Members.Where(b => b.Id == senderProfile.Member.Id).Select(y => y).FirstOrDefault();
            var groupCalendar = db.GroupCalendars.Where(g => g.RequestId == syncRequest.Id).Select(w => w).FirstOrDefault();
            foreach(var request in member.GroupSyncRequests)
            {
                if(request.Id == syncRequest.Id)
                {
                    member.GroupSyncRequests.Remove(request);
                    break;
                }
            }
            groupCalendar.Members.Add(member);
            member.GroupCalendars.Add(groupCalendar);
            db.SaveChanges();
            TempData["Message"] = "**You have a new synced group calendar!";
            return RedirectToAction("ViewGroupSyncRequests");
        }

        public ActionResult DenySyncRequest(int? id)
        {
            var user = User.Identity.GetUserId();
            var member = db.Members.Where(u => u.UserId.Id == user).Select(s => s).FirstOrDefault();
            var syncRequest = db.SyncRequests.Where(n => n.Id == id).Select(o => o).FirstOrDefault();
            syncRequest.Status = "Denied";
            member.SyncRequests.Remove(syncRequest);
            db.SyncRequests.Remove(syncRequest);
            db.SaveChanges();
            TempData["Message"] = "**Sync request was removed from your pending sync requests.";
            return RedirectToAction("ViewSyncRequests");
        }

        public ActionResult DenyGroupSyncRequest(int? id)
        {
            var user = User.Identity.GetUserId();
            var member = db.Members.Where(u => u.UserId.Id == user).Select(s => s).FirstOrDefault();
            var syncRequest = db.GroupSyncRequests.Where(n => n.Id == id).Select(o => o).FirstOrDefault();
            foreach(var sync in member.GroupSyncRequests)
            {
                if(sync.Id == syncRequest.Id)
                {
                    member.GroupSyncRequests.Remove(sync);
                }
            }
            db.SaveChanges();
            TempData["Message"] = "**Sync request was removed from your pending group sync requests.";
            return RedirectToAction("ViewGroupSyncRequests");
        }

        public ActionResult ChooseSyncedCalendar()
        {
            var current = User.Identity.GetUserId();
            var member = db.Members.Where(m => m.UserId.Id == current).Select(s => s).FirstOrDefault();
            if (member.SyncRequests.Count == 0)
            {
                TempData["ErrorMessage"] = "**You currently don't have any synced calendars to view...";
                return RedirectToAction("ViewCalendar");
            }
            var syncedCalendars = new List<Profile>();
            foreach (var sync in member.SyncRequests)
            {
                if(sync.Status == "Approved")
                {
                    var profile = db.Profiles.Where(p => p.Id == sync.Sender.Id).Select(a => a).FirstOrDefault();
                    syncedCalendars.Add(profile);
                }
            }
            if(syncedCalendars.Count == 0)
            {
                TempData["ErrorMessage"] = "**You currently don't have any synced calendars to view...";
                return RedirectToAction("ViewCalendar");
            }
            return View(syncedCalendars.OrderBy(o => o.LastName));
        }

        public ActionResult ChooseGroupSyncedCalendar()
        {
            var current = User.Identity.GetUserId();
            var member = db.Members.Where(m => m.UserId.Id == current).Select(s => s).FirstOrDefault();
            var profile = db.Profiles.Where(p => p.Member.Id == member.Id).Select(a => a).FirstOrDefault();
            if (member.GroupCalendars.Count == 0)
            {
                TempData["ErrorMessage"] = "**You currently don't have any group calendars to view...";
                return RedirectToAction("ViewCalendar");
            }
            var groupCalendars = new List<GroupCalendar>();
            foreach (var calendar in member.GroupCalendars)
            {
                groupCalendars.Add(calendar);
            }
            return View(groupCalendars.OrderBy(o => o.Date));
        }

        public ActionResult ViewSyncedCalendar(int? id)
        {
            var current = User.Identity.GetUserId();
            var member = db.Members.Where(m => m.UserId.Id == current).Select(s => s).FirstOrDefault();
            var profile = db.Profiles.Where(l => l.Member.Id == member.Id).Select(b => b).FirstOrDefault();
            var otherProfile = db.Profiles.Where(p => p.Id == id).Select(a => a).FirstOrDefault();
            var otherMember = db.Members.Where(o => o.Id == otherProfile.Member.Id).Select(t => t).FirstOrDefault();
            var events1 = new List<Event>();
            var events2 = new List<Event>();
            foreach(var @event in member.Events)
            {
                if(@event.isPrivate == false)
                {
                    events1.Add(@event);
                }
            }
            foreach (var @event2 in otherMember.Events)
            {
                if (@event2.isPrivate == false)
                {
                    events2.Add(@event2);
                }
            }
            var viewModel = new EventsViewModel();
            viewModel.ListA = events1;
            viewModel.ListB = events2;
            ViewBag.Name1 = otherProfile.FirstName;
            ViewBag.Name2 = otherProfile.LastName;
            return View(viewModel);
        }

        public ActionResult ViewGroupSyncedCalendar(int? id)
        {
            var current = User.Identity.GetUserId();
            var member = db.Members.Where(m => m.UserId.Id == current).Select(s => s).FirstOrDefault();
            var groupSynced = db.GroupCalendars.Where(g => g.Id == id).Select(a => a).FirstOrDefault();
            var countMembers = new List<Member>();
            foreach(var person in groupSynced.Members)
            {
                countMembers.Add(person);
            }
            if (countMembers.Count >= 1)
            {
                var events1 = new List<Event>();
                var events2 = new List<Event>();
                var events3 = new List<Event>();
                var events4 = new List<Event>();
                var events5 = new List<Event>();
                for (int i = 0; i < countMembers.Count; i++)
                {
                    var memberId = countMembers[i].Id;
                    if (countMembers[i] == countMembers[0])
                    {
                        var otherMember = db.Members.Where(o => o.Id == memberId).Select(t => t).FirstOrDefault();
                        var profile = db.Profiles.Where(p => p.Member.Id == otherMember.Id).Select(h => h).FirstOrDefault();
                        ViewBag.Name1 = profile.FirstName;
                        ViewBag.Name11 = profile.LastName;
                        foreach (var @event2 in otherMember.Events)
                        {
                            if (@event2.isPrivate == false)
                            {
                                events1.Add(@event2);
                            }
                        }
                    }
                    else if(countMembers[i] == countMembers[1])
                    {
                        var otherMember = db.Members.Where(o => o.Id == memberId).Select(t => t).FirstOrDefault();
                        var profile = db.Profiles.Where(p => p.Member.Id == otherMember.Id).Select(h => h).FirstOrDefault();
                        ViewBag.Name2 = profile.FirstName;
                        ViewBag.Name22 = profile.LastName;
                        foreach (var @event2 in otherMember.Events)
                        {
                            if (@event2.isPrivate == false)
                            {
                                events2.Add(@event2);
                            }
                        }
                    }
                    else if (countMembers[i] == countMembers[2])
                    {
                        var otherMember = db.Members.Where(o => o.Id == memberId).Select(t => t).FirstOrDefault();
                        var profile = db.Profiles.Where(p => p.Member.Id == otherMember.Id).Select(h => h).FirstOrDefault();
                        ViewBag.Name3 = profile.FirstName;
                        ViewBag.Name33 = profile.LastName;
                        foreach (var @event2 in otherMember.Events)
                        {
                            if (@event2.isPrivate == false)
                            {
                                events3.Add(@event2);
                            }
                        }
                    }
                    else if (countMembers[i] == countMembers[3])
                    {
                        var otherMember = db.Members.Where(o => o.Id == memberId).Select(t => t).FirstOrDefault();
                        var profile = db.Profiles.Where(p => p.Member.Id == otherMember.Id).Select(h => h).FirstOrDefault();
                        ViewBag.Name4 = profile.FirstName;
                        ViewBag.Name44 = profile.LastName;
                        foreach (var @event2 in otherMember.Events)
                        {
                            if (@event2.isPrivate == false)
                            {
                                events4.Add(@event2);
                            }
                        }
                    }
                    else if (countMembers[i] == countMembers[4])
                    {
                        var otherMember = db.Members.Where(o => o.Id == memberId).Select(t => t).FirstOrDefault();
                        var profile = db.Profiles.Where(p => p.Member.Id == otherMember.Id).Select(h => h).FirstOrDefault();
                        ViewBag.Name5 = profile.FirstName;
                        ViewBag.Name55 = profile.LastName;
                        foreach (var @event2 in otherMember.Events)
                        {
                            if (@event2.isPrivate == false)
                            {
                                events5.Add(@event2);
                            }
                        }
                    }
                }
                var viewModel = new EventsViewModel();
                viewModel.ListA = events1;
                viewModel.ListB = events2;
                viewModel.ListC = events3;
                viewModel.ListD = events4;
                viewModel.ListE = events5;
                return View(viewModel);
            }
            TempData["ErrorMessage"] = "**An unknown error occured while retrieving your group synced calendar.";
            return RedirectToAction("ViewCalendar");
        }

        public ActionResult ViewMySyncedCalendar(string address)
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
                ViewBag.Address = address;
                return View(events);
            }
            catch
            {
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index", "Users");
                }
                else
                {
                    TempData["ErrorMessage"] = "**An unknown error occured while syncing your calendar.";
                    return RedirectToAction("ViewCalendar");
                }
            }
        }

        public ActionResult RemoveSyncedCalendar(int? id) 
        {
            var user = User.Identity.GetUserId();
            var member = db.Members.Where(u => u.UserId.Id == user).Select(s => s).FirstOrDefault();
            var syncRequest = db.SyncRequests.Where(n => n.Sender.Id == id).Select(o => o).FirstOrDefault();
            var syncRequest2 = db.SyncRequests.Where(k => k.Receiver.Id == id).Select(t => t).FirstOrDefault(); 
            var otherProfile = db.Profiles.Where(b => b.Id == syncRequest.Sender.Id).Select(n => n).FirstOrDefault();
            var otherMember = db.Members.Where(v => v.Id == otherProfile.Member.Id).Select(e => e).FirstOrDefault();
            member.SyncRequests.Remove(syncRequest);
            otherMember.SyncRequests.Remove(syncRequest2);
            db.SyncRequests.Remove(syncRequest2);
            db.SyncRequests.Remove(syncRequest);
            db.SaveChanges();
            TempData["Message"] = "**Synced calendar was successfully removed.";
            return RedirectToAction("ViewSyncRequests");
        }

        public ActionResult RemoveGroupSyncedCalendar(int? id)
        {
            var user = User.Identity.GetUserId();
            var member = db.Members.Where(u => u.UserId.Id == user).Select(s => s).FirstOrDefault();
            var memberProfile = db.Profiles.Where(m => m.Member.Id == member.Id).Select(a => a).FirstOrDefault();
            var calendar = db.GroupCalendars.Where(n => n.Id == id).Select(o => o).FirstOrDefault();
            member.GroupCalendars.Remove(calendar);
            foreach(var person in calendar.Members)
            {
                if(person.Id == member.Id)
                {
                    calendar.Members.Remove(person);
                    break;
                }
            }
            if(calendar.Members.Count == 0)
            {
                db.GroupCalendars.Remove(calendar);
            }
            db.SaveChanges();
            TempData["Message"] = "**You are no longer a member of a group calendar.";
            return RedirectToAction("ChooseGroupSyncedCalendar");
        }

        public ActionResult AddToMyEvents(int? id)
        {
            var user = User.Identity.GetUserId();
            var member = db.Members.Where(u => u.UserId.Id == user).Select(s => s).FirstOrDefault();
            var @event = db.Events.Where(e => e.Id == id).Select(y => y).FirstOrDefault();
            //var otherMember = db.Members.Where(o => o.Id == @event.Member_Id).Select(b => b).FirstOrDefault();
            //var otherProfile = db.Profiles.Where(p => p.Member.Id == otherMember.Id).Select(t => t).FirstOrDefault();
            var newEvent = new Event();
            newEvent.title = @event.title;
            newEvent.streetAddress = @event.streetAddress;
            newEvent.city = @event.city;
            newEvent.state = @event.state;
            newEvent.zipCode = @event.zipCode;
            newEvent.start = @event.start;
            newEvent.end = @event.end;
            newEvent.startTime = @event.startTime;
            newEvent.endTime = @event.endTime;
            newEvent.details = @event.details;
            member.Events.Add(newEvent);
            db.SaveChanges();
            TempData["Message"] = "**You have a new event!";
            //return RedirectToAction("ViewSyncedCalendar", new { id = otherProfile.Id });
            return RedirectToAction("ViewCalendar");
        }

        public ActionResult RemoveAllSyncRequests()
        {
            if(db.SyncRequests.ToList().Count == 0)
            {
                return RedirectToAction("Index", "Users");
            }
            foreach(var request in db.SyncRequests.ToList())
            {
                db.SyncRequests.Remove(request);
            }
            foreach(var member in db.Members.ToList())
            {
                foreach(var sync in member.SyncRequests)
                {
                    member.SyncRequests.Remove(sync);
                }
            }
            db.SaveChanges();
            TempData["Message"] = "**All sync requests deleted.";
            return RedirectToAction("Index", "Users");
        }

        public ActionResult RemoveAllGroupSyncRequests()
        {
            if (db.GroupSyncRequests.ToList().Count == 0)
            {
                return RedirectToAction("Index", "Users");
            }
            foreach (var member in db.Members.ToList())
            {
                foreach (var sync in member.GroupSyncRequests.ToList())
                {
                    member.GroupSyncRequests.Remove(sync);
                }
            }
            foreach (var request in db.GroupSyncRequests.ToList())
            {
                db.GroupSyncRequests.Remove(request);
            }
            db.SaveChanges();
            TempData["Message"] = "**All group sync requests deleted.";
            return RedirectToAction("Index", "Users");
        }

        public ActionResult RemoveAllGroupCalendars()
        {
            if (db.GroupCalendars.ToList().Count == 0)
            {
                return RedirectToAction("Index", "Users");
            }
            foreach (var member in db.Members.ToList())
            {
                foreach (var sync in member.GroupCalendars.ToList())
                {
                    member.GroupCalendars.Remove(sync);
                }
            }
            foreach (var calendar in db.GroupCalendars.ToList())
            {
                db.GroupCalendars.Remove(calendar);
            }
            db.SaveChanges();
            TempData["Message"] = "**All group calendars deleted.";
            return RedirectToAction("Index", "Users");
        }

        public ActionResult GetGoogleCalendarId()
        {
            return View();
        }

        public ActionResult SyncGoogleCalendar(string address)
        {
            var user = User.Identity.GetUserId();
            var member = db.Members.Where(u => u.UserId.Id == user).Select(s => s).FirstOrDefault();
            var memberProfile = db.Profiles.Where(q => q.Member.Id == member.Id).Select(e => e).FirstOrDefault();
            if(address == null || address == "")
            {
                TempData["ErrorMessage"] = "**You must input a valid Google Calendar ID.";
                return RedirectToAction("GetGoogleCalendarId");
            }
            var calendarAddress = address;
            db.SaveChanges();
            TempData["Message"] = "**You successfully synced your Google Calendar!";
            return RedirectToAction("ViewMySyncedCalendar", new { address = calendarAddress });
        }
    }
}
