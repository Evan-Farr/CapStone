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
    public class SyncRequestsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: SyncRequests
        public ActionResult Index()
        {
            return View(db.SyncRequests.ToList());
        }

        // GET: SyncRequests/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SyncRequest syncRequest = db.SyncRequests.Find(id);
            if (syncRequest == null)
            {
                return HttpNotFound();
            }
            return View(syncRequest);
        }

        // GET: SyncRequests/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SyncRequests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,DateSent,Status")] SyncRequest syncRequest)
        {
            if (ModelState.IsValid)
            {
                db.SyncRequests.Add(syncRequest);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(syncRequest);
        }

        // GET: SyncRequests/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SyncRequest syncRequest = db.SyncRequests.Find(id);
            if (syncRequest == null)
            {
                return HttpNotFound();
            }
            return View(syncRequest);
        }

        // POST: SyncRequests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,DateSent,Status")] SyncRequest syncRequest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(syncRequest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(syncRequest);
        }

        // GET: SyncRequests/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SyncRequest syncRequest = db.SyncRequests.Find(id);
            if (syncRequest == null)
            {
                return HttpNotFound();
            }
            return View(syncRequest);
        }

        // POST: SyncRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SyncRequest syncRequest = db.SyncRequests.Find(id);
            db.SyncRequests.Remove(syncRequest);
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
