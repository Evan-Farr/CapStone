using SyncMe.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SyncMe.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        // GET: Users
        public ActionResult Index()
        {
            ApplicationDbContext db = new ApplicationDbContext();

            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                ViewBag.Name = user.Name;
                ViewBag.displayMenu = "No";
                if (isUser("Admin"))
                {
                    ViewBag.displayMenu = "Yes";
                }
                else if (isUser("Member"))
                {
                    string holder = user.GetUserId();
                    var temp = db.Members.Where(i => i.UserId.Id == holder).FirstOrDefault().Id;
                    var member = db.Members.Where(b => b.UserId.Id == holder).Select(q => q).FirstOrDefault();
                    ViewBag.Id = temp;
                    List<SelectListItem> contacts = new List<SelectListItem>();
                    foreach (var contact in member.Contacts)
                    {
                        contacts.Add(new SelectListItem { Text = contact.FirstName + " " + contact.LastName, Value = contact.Id.ToString() });
                    }
                    ViewBag.Contacts = contacts;
                    ViewBag.displayMenu = "Member";
                }
                return View();
            }
            else
            {
                ViewBag.Name = "Not Logged In";
            }
            return View();
        }

        public bool isUser(string role)
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