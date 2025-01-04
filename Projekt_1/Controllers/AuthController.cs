using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Projekt_1.Model;

namespace Projekt_1.Controllers
{
    public class AuthController : Controller
    {
        private Project1DBEntities db = new Project1DBEntities();

        // GET: Auth/Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                TempData["ErrorMessage"] = "Email and password are required.";
                return View();
            }

            if (!IsValidEmail(email))
            {
                TempData["ErrorMessage"] = "Invalid email format.";
                return View();
            }

            var hashedPassword = HashPassword(password);
            var user = db.users.FirstOrDefault(u => u.email == email && u.password == hashedPassword);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Invalid email or password.";
                return View();
            }

            

            // Set session or authentication logic
            Session["UserId"] = user.user_id;
            Session["UserEmail"] = user.email;
            Session["UserEmail"] = user.role;


            TempData["SuccessMessage"] = "Login successful!";

            if (user.role == "Admin")
            {
                return RedirectToAction("Index", "SavingsAccountTypes"); // Redirect to Admin controller
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: Auth/Register
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "email,password,user_name,user_birth,user_address,user_identity,user_phone")] user user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!IsValidEmail(user.email))
                    {
                        TempData["ErrorMessage"] = "Invalid email format.";
                        return View(user);
                    }

                    if (!IsStrongPassword(user.password))
                    {
                        TempData["ErrorMessage"] = "Password must be at least 8 characters long and include uppercase, lowercase, and special characters.";
                        return View(user);
                    }

                    if (db.users.Any(u => u.email == user.email))
                    {
                        TempData["ErrorMessage"] = "Email already exists.";
                        return View(user);
                    }

                    // Gán giá trị mặc định nếu cần
                    user.user_birth = user.user_birth ?? DateTime.Now;
                    user.user_address = user.user_address ?? "N/A";
                    user.user_identity = user.user_identity ?? 0;
                    user.user_phone = user.user_phone ?? 0;
                    user.role = "User"; // Gán giá trị mặc định cho role

                    user.password = HashPassword(user.password);
                    db.users.Add(user);
                    db.SaveChanges();

                    TempData["SuccessMessage"] = "Registration successful! You can now log in.";
                    return RedirectToAction("Login");
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                    }
                }

                TempData["ErrorMessage"] = "An error occurred while registering. Please check your inputs.";
            }

            return View(user);
        }

        // GET: Auth/Logout
        public ActionResult Logout()
        {
            Session.Clear();
            TempData["SuccessMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Login");
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private bool IsStrongPassword(string password)
        {
            return password.Length >= 8
                   && password.Any(char.IsUpper)
                   && password.Any(char.IsLower)
                   && password.Any(char.IsDigit)
                   && password.Any(ch => !char.IsLetterOrDigit(ch));
        }

        // Dispose
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
