using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SyncMe.Models;

namespace SyncMe.Controllers
{
    public class EventInvitationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: EventInvitations
        public ActionResult Index()
        {
            return View(db.EventInvitations.ToList());
        }

        // GET: EventInvitations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventInvitation eventInvitation = db.EventInvitations.Find(id);
            if (eventInvitation == null)
            {
                return HttpNotFound();
            }
            return View(eventInvitation);
        }

        // GET: EventInvitations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EventInvitations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Date,Status")] EventInvitation eventInvitation)
        {
            if (ModelState.IsValid)
            {
                db.EventInvitations.Add(eventInvitation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(eventInvitation);
        }

        // GET: EventInvitations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventInvitation eventInvitation = db.EventInvitations.Find(id);
            if (eventInvitation == null)
            {
                return HttpNotFound();
            }
            return View(eventInvitation);
        }

        // POST: EventInvitations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Date,Status")] EventInvitation eventInvitation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(eventInvitation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(eventInvitation);
        }

        // GET: EventInvitations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventInvitation eventInvitation = db.EventInvitations.Find(id);
            if (eventInvitation == null)
            {
                return HttpNotFound();
            }
            return View(eventInvitation);
        }

        // POST: EventInvitations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EventInvitation eventInvitation = db.EventInvitations.Find(id);
            db.EventInvitations.Remove(eventInvitation);
            db.SaveChanges();
            return RedirectToAction("Index");
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
