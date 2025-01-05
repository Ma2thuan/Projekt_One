using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Projekt_1.Model;

namespace Projekt_1.Controllers
{
    public class HomeController : Controller
    {
        private Project1DBEntities db = new Project1DBEntities();


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult login()
        {
            return View("login");
        }
        public ActionResult register()
        {
            return View("register");
        }

        public ActionResult chart()
        {
            return View("charts");
        }

        public ActionResult light_nav()
        {
            return View("layout-sidenav-light");

        }

        public ActionResult static_nav()
        {
            return View("layout-static");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Logout()
        {
            // Clear session
            Session.Clear();

            // Redirect to Login in AuthController
            return RedirectToAction("Login", "Auth");
        }

        // GET: Home/AccountSettings
        public ActionResult AccountSettings()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            int userId = Convert.ToInt32(Session["UserId"]);
            var user = db.users.FirstOrDefault(u => u.user_id == userId);

            if (user == null)
            {
                return HttpNotFound("User not found.");
            }

            return View(user);
        }

        // POST: Home/UpdateUserInfo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateUserInfo(int user_id, string user_name, string email, DateTime? user_birth, string user_address, string user_phone)
        {
            if (Session["UserId"] == null || Convert.ToInt32(Session["UserId"]) != user_id)
            {
                return new HttpStatusCodeResult(403, "Unauthorized");
            }

            var user = db.users.FirstOrDefault(u => u.user_id == user_id);
            if (user == null)
            {
                return HttpNotFound("User not found.");
            }

            user.user_name = user_name;
            user.email = email;
            user.user_birth = user_birth;
            user.user_address = user_address;
            user.user_phone = !string.IsNullOrEmpty(user_phone) ? (int?)Convert.ToInt32(user_phone) : null;

            db.SaveChanges();

            TempData["SuccessMessage"] = "Information updated successfully.";
            return RedirectToAction("AccountSettings");
        }

    }
}