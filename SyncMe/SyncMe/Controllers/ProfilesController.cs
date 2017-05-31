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
using System.IO;
using System.Web.Helpers;

namespace SyncMe.Controllers
{
    public class ProfilesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Profiles
        public ActionResult Index()
        {
            return View(db.Profiles.ToList());
        }

        public ActionResult Search()
        {
            var user = User.Identity.GetUserId();
            var member = db.Members.Where(u => u.UserId.Id == user).Select(s => s).FirstOrDefault();
            var profiles = db.Profiles.ToList();
            var Profiles = new List<Profile>();
            foreach(var p in profiles)
            {
                if(p.Member.Id != member.Id)
                {
                    Profiles.Add(p);
                }
            }
            var viewProfile = db.Profiles.Where(v => v.Member.Id == member.Id).Select(y => y).FirstOrDefault();
            ViewBag.counter = 0;
            ViewBag.Id = viewProfile.Id;
            ViewBag.Contacts = member.Contacts;
            ViewBag.ContactRequests = member.ContactRequests;
            ViewBag.AllContactRequests = db.ContactRequests.ToList();
            return View(Profiles.OrderBy(o => o.LastName));
        }

        public ActionResult SearchResults(string name)
        {
            var user = User.Identity.GetUserId();
            var member = db.Members.Where(u => u.UserId.Id == user).Select(s => s).FirstOrDefault();
            var profiles = db.Profiles.ToList();
            var Profiles = new List<Profile>();
            foreach (var profile in profiles)
            {
                if (profile.Member.Id != member.Id)
                {
                    if(profile.FirstName.ToLower().Contains(name.ToLower()))
                    {
                        Profiles.Add(profile);
                    }else if (profile.LastName.ToLower().Contains(name.ToLower()))
                    {
                        Profiles.Add(profile);
                    }
                }
            }
            if (Profiles.Count == 0)
            {
                TempData["ErrorMessage"] = "**No SyncMe matches were found for that search....";
                return RedirectToAction("Search");
            }
            var viewProfile = db.Profiles.Where(v => v.Member.Id == member.Id).Select(y => y).FirstOrDefault();
            ViewBag.counter = 0;
            ViewBag.Id = viewProfile.Id;
            ViewBag.Contacts = member.Contacts.ToList();
            ViewBag.ContactRequests = member.ContactRequests.ToList();
            ViewBag.AllContactRequests = db.ContactRequests.ToList();
            return View(Profiles.OrderBy(m => m.LastName));
        }

        // GET: Profiles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = db.Profiles.Find(id);
            if (profile == null)
            {
                return HttpNotFound();
            }
            return View(profile);
        }

        public ActionResult PrivateDetails()
        {
            var current = User.Identity.GetUserId();
            var member = db.Members.Where(m => m.UserId.Id == current).Select(s => s).FirstOrDefault();
            var profile = db.Profiles.Where(p => p.Member.Id == member.Id).Select(a => a).FirstOrDefault();
            return View(profile);
        }

        // GET: Profiles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Profiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProfilePictureId,FirstName,LastName,Age,State,CompanyName,SchoolName,Phone,Email")] Profile profile, HttpPostedFileBase file1)
        {
            if (ModelState.IsValid)
            {
                var current = User.Identity.GetUserId();
                var member = db.Members.Where(m => m.UserId.Id == current).Select(s => s).FirstOrDefault();
                profile.Member = member;
                string fileName = "";
                byte[] bytes;
                int bytesToRead;
                int numBytesRead;
                if (file1 != null)
                {
                    fileName = Path.GetFileName(file1.FileName);
                    bytes = new byte[file1.ContentLength];
                    bytesToRead = (int)file1.ContentLength;
                    numBytesRead = 0;
                    while(bytesToRead > 0)
                    {
                        int n = file1.InputStream.Read(bytes, numBytesRead, bytesToRead);
                        if(n == 0) { break; }
                        numBytesRead += n;
                        bytesToRead -= n;
                    }
                    profile.ProfilePictureId = bytes;
                }
                db.Profiles.Add(profile);
                db.SaveChanges();
                return RedirectToAction("ViewCalendar", "Members");
            }
            return View(profile);
        }

        // GET: Profiles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = db.Profiles.Find(id);
            if (profile == null)
            {
                return HttpNotFound();
            }
            return View(profile);
        }

        // POST: Profiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProfilePictureId,FirstName,LastName,Age,State,CompanyName,SchoolName,Phone,Email")] Profile profile, HttpPostedFileBase file2)
        {
            if (ModelState.IsValid)
            {
                var current = User.Identity.GetUserId();
                var member = db.Members.Where(m => m.UserId.Id == current).Select(s => s).FirstOrDefault();
                string fileName = "";
                byte[] bytes;
                int bytesToRead;
                int numBytesRead;
                if (file2 != null)
                {
                    fileName = Path.GetFileName(file2.FileName);
                    bytes = new byte[file2.ContentLength];
                    bytesToRead = (int)file2.ContentLength;
                    numBytesRead = 0;
                    while (bytesToRead > 0)
                    {
                        int n = file2.InputStream.Read(bytes, numBytesRead, bytesToRead);
                        if (n == 0) { break; }
                        numBytesRead += n;
                        bytesToRead -= n;
                    }
                    profile.ProfilePictureId = bytes;
                    if(db.Contacts.ToList().Count != 0)
                    {
                        foreach (var contact in db.Contacts.ToList())
                        {
                            if (contact.ContactId == profile.Id)
                            {
                                contact.ProfilePictureId = profile.ProfilePictureId;
                                contact.FirstName = profile.FirstName;
                                contact.LastName = profile.LastName;
                                contact.Age = profile.Age;
                                contact.State = profile.State;
                                contact.CompanyName = profile.CompanyName;
                                contact.SchoolName = profile.SchoolName;
                                contact.Phone = profile.Phone;
                                contact.Email = profile.Email;
                            }
                        }
                    }
                    db.Entry(profile).State = EntityState.Modified;
                    db.SaveChanges();
                    if (User.IsInRole("Admin")) { return RedirectToAction("Details", new { id = profile.Id }); }
                    return RedirectToAction("PrivateDetails");
                }
                if (db.Contacts.ToList().Count != 0)
                {
                    foreach (var contact in db.Contacts.ToList())
                    {
                        if (contact.ContactId == profile.Id)
                        {
                            contact.ProfilePictureId = profile.ProfilePictureId;
                            contact.FirstName = profile.FirstName;
                            contact.LastName = profile.LastName;
                            contact.Age = profile.Age;
                            contact.State = profile.State;
                            contact.CompanyName = profile.CompanyName;
                            contact.SchoolName = profile.SchoolName;
                            contact.Phone = profile.Phone;
                            contact.Email = profile.Email;
                        }
                    }
                }
                db.Entry(profile).State = EntityState.Modified;
                db.SaveChanges();
                if (User.IsInRole("Admin")) { return RedirectToAction("Details", new { id = profile.Id }); }
                return RedirectToAction("PrivateDetails");
            }
            return View(profile);
        }

        // GET: Profiles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = db.Profiles.Find(id);
            if (profile == null)
            {
                return HttpNotFound();
            }
            return View(profile);
        }

        // POST: Profiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Profile profile = db.Profiles.Find(id);
            var contact = db.Contacts.Where(c => c.ContactId == profile.Id).Select(s => s).FirstOrDefault();
            foreach(var member in db.Members.ToList())
            {
                foreach(var contact2 in member.Contacts)
                {
                    if(contact2.ContactId == contact.ContactId)
                    {
                        member.Contacts.Remove(contact2);
                        break;
                    }
                }
            }
            db.Contacts.Remove(contact);
            db.Profiles.Remove(profile);
            db.SaveChanges();
            if (User.IsInRole("Admin")) { TempData["Message"] = "**Profile successfully deleted."; return RedirectToAction("Index", "Users"); }
            TempData["Message"] = "**Your Profile has successfully been deleted.";
            return RedirectToAction("ViewCalendar", "Members");
        }

        public void SelectPicture(int? id)
        {
            //var current = User.Identity.GetUserId();
            //var member = db.Members.Where(m => m.UserId.Id == current).Select(s => s).FirstOrDefault();
            var profile = db.Profiles.Where(p => p.Id == id).Select(a => a).FirstOrDefault();
            try
            {
                WebImage wi = new WebImage(profile.ProfilePictureId);
                wi.Resize(200, 200, true, true);
                wi.Write();
            }catch
            {
                WebImage wiDefault = new WebImage("~/App_Data/uploads/no-profile-image.jpg");
                wiDefault.Resize(200, 200, true, true);
                wiDefault.Write();
            }
        }

        public void SelectPictureContact(int? id)
        {
            //var current = User.Identity.GetUserId();
            //var member = db.Members.Where(m => m.UserId.Id == current).Select(s => s).FirstOrDefault();
            var contact = db.Contacts.Where(p => p.Id == id).Select(a => a).FirstOrDefault();
            try
            {
                WebImage wi = new WebImage(contact.ProfilePictureId);
                wi.Resize(200, 200, true, true);
                wi.Write();
            }
            catch
            {
                WebImage wiDefault = new WebImage("~/App_Data/uploads/no-profile-image.jpg");
                wiDefault.Resize(200, 200, true, true);
                wiDefault.Write();
            }
        }

        public void SelectAllPictures()
        {
            foreach(var profile in db.Profiles)
            {
                try
                {
                    WebImage wi = new WebImage(profile.ProfilePictureId);
                    wi.Resize(100, 100, true, true);
                    wi.Write();
                }
                catch
                {
                    WebImage wiDefault = new WebImage("~/App_Data/uploads/no-profile-image.jpg");
                    wiDefault.Resize(100, 100, true, true);
                    wiDefault.Write();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
