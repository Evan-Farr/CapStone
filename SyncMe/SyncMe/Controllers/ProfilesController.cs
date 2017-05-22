﻿using System;
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
        public ActionResult Create([Bind(Include = "Id,ProfilePictureId,CompanyName,SchoolName,Phone,Email")] Profile profile, HttpPostedFileBase file1)
        {
            if (ModelState.IsValid)
            {
                var current = User.Identity.GetUserId();
                var member = db.Members.Where(m => m.UserId.Id == current).Select(s => s).FirstOrDefault();
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
                profile.Member = member;
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
        public ActionResult Edit([Bind(Include = "Id,ProfilePictureId,CompanyName,SchoolName,Phone,Email")] Profile profile, HttpPostedFileBase file2)
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
                    db.Entry(profile).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("PrivateDetails");
                }
                profile.ProfilePictureId = profile.ProfilePictureId;
                db.Entry(profile).State = EntityState.Modified;
                db.SaveChanges();
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
            db.Profiles.Remove(profile);
            db.SaveChanges();
            return RedirectToAction("Index");
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
                return RedirectToAction("ViewContacts", "Members");
            }
            var current = User.Identity.GetUserId();
            var member = db.Members.Where(m => m.UserId.Id == current).Select(s => s).FirstOrDefault();
            member.Contacts.Remove(profile);
            TempData["Message"] = "**SyncMe member successfully removed from your contacts.";
            return RedirectToAction("ViewContacts", "Members");
        }

        public void SelectPicture()
        {
            var current = User.Identity.GetUserId();
            var member = db.Members.Where(m => m.UserId.Id == current).Select(s => s).FirstOrDefault();
            var profile = db.Profiles.Where(p => p.Member.Id == member.Id).Select(a => a).FirstOrDefault();
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
