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

        public ActionResult AccountSettings()
        {
            // Kiểm tra xem người dùng đã đăng nhập chưa
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Auth"); // Chuyển hướng về trang Login nếu chưa đăng nhập
            }
            // Lấy UserId từ Session
            int userId = Convert.ToInt32(Session["UserId"]);
            // Lấy thông tin người dùng từ cơ sở dữ liệu
            var user = db.users.FirstOrDefault(u => u.user_id == userId);
            if (user == null)
            {
                return HttpNotFound("User not found.");
            }
            return View(user);
        }
    }
}