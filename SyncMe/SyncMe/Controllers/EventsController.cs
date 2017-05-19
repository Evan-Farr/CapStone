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
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Routing;
using System.Globalization;

namespace SyncMe.Controllers
{
    public class EventsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Events
        public ActionResult Index()
        {
            return View(db.Events.ToList());
        }

        // GET: Events/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // GET: Events/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,title,streetAddress,city,state,zipCode,start,end,startTime,endTime,details,isPrivate")] Event @event, DateTime StartDate, DateTime EndDate, DateTime StartTime, DateTime EndTime)
        {
            if (ModelState.IsValid)
            {
                @event.start = StartDate.Date.Add(DateTime.Parse(StartTime));
                @event.end = EndDate.ToString("yyyy-MM-dd ") + EndTime.ToString("HH:mm:ss");
                string startDate = StartDate.ToString("yyyy-MM-dd");
                string endDate = EndDate.ToString("yyyy-MM-dd");
                string startTime = StartTime.ToString("HH:mm:ss");
                string endTime = EndTime.ToString("HH:mm:ss");
                @event.start = DateTime.ParseExact(startDate + " " + startTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                @event.end = DateTime.ParseExact(endDate + " " + endTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                var holder = User.Identity.GetUserId();
                var member = db.Members.Where(u => u.UserId.Id == holder).Select(s => s).FirstOrDefault();
                member.Events.Add(@event);
                db.Events.Add(@event);
                db.SaveChanges();
                return RedirectToAction("ViewCalendar", new RouteValueDictionary(new { controller = "Members", action = "ViewCalendar" }));
            }
            return View(@event);
        }

        // GET: Events/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,title,streetAddress,city,state,zipCode,start,end,startTime,endTime,details,isPrivate")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@event).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ViewCalendar", new RouteValueDictionary(new { controller = "Members", action = "ViewCalendar" }));
            }
            return View(@event);
        }

        // GET: Events/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Event @event = db.Events.Find(id);
            db.Events.Remove(@event);
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

        private bool isUser(string role)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                ApplicationDbContext context = new ApplicationDbContext();
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var s = UserManager.GetRoles(user.GetUserId());
                if (s[0].ToString() == role)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
    }
}
