using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Projekt_1.Model;

namespace Projekt_1.Controllers
{
    public class passbooksController : Controller
    {
        private Project1DBEntities db = new Project1DBEntities();

        // Dùng để khi chọn tháng gửi tiền lãi suất sẽ lock in
        public JsonResult GetInterestRate(int savingsTypeId)
        {
            var interestRate = db.SavingsAccountTypes
                                 .Where(s => s.SavingsTypeID == savingsTypeId)
                                 .Select(s => s.InterestRate)
                                 .FirstOrDefault();

            return Json(interestRate, JsonRequestBehavior.AllowGet);
        }

        // GET: passbooks
        public ActionResult Index()
        {
            var passbooks = db.passbooks.Include(p => p.SavingsAccountType).Include(p => p.user);
            return View(passbooks.ToList());
        }

        //public ActionResult Search(int? initialDepositAmount, string? term)
        //{
        //    var passbooks = db.passbooks.Include(p => p.SavingsAccountType).Include(p => p.user);

        //    if (initialDepositAmount.HasValue)
        //    {
        //        passbooks = passbooks.Where(p => p.InitialDepositAmount <= initialDepositAmount.Value);
        //    }

        //    if (depositAmount.HasValue)
        //    {
        //        passbooks = passbooks.Where(p => p.DepositAmount <= depositAmount.Value);
        //    }

        //    ViewBag.InitialDepositAmount = initialDepositAmount;
        //    ViewBag.DepositAmount = depositAmount;

        //    return View("Index",passbooks.ToList());
        //}

        public ActionResult Search(int? savingsBookID, int? initialDepositAmount, string term)
        {
            var passbooks = db.passbooks.Include(p => p.SavingsAccountType).Include(p => p.user);

            if (savingsBookID.HasValue)
            {
                passbooks = passbooks.Where(p => p.SavingsBookID == savingsBookID.Value);
            }

            if (initialDepositAmount.HasValue)
            {
                passbooks = passbooks.Where(p => p.InitialDepositAmount <= initialDepositAmount.Value);
            }

            if (!string.IsNullOrEmpty(term))
            {
                passbooks = passbooks.Where(p => p.user.user_name.Contains(term) || p.SavingsAccountType.AccountTypeName.Contains(term));
            }

            // Return the main view
            ViewBag.SavingsBookID = savingsBookID;
            ViewBag.InitialDepositAmount = initialDepositAmount;
            ViewBag.Term = term;

            return View("Index", passbooks.ToList());
        }


        // GET: passbooks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            passbook passbook = db.passbooks.Find(id);
            if (passbook == null)
            {
                return HttpNotFound();
            }
            return View(passbook);
        }

        // GET: passbooks/Create
        public ActionResult Create()
        {
            ViewBag.SavingsType = new SelectList(db.SavingsAccountTypes, "SavingsTypeID", "AccountTypeName");
            ViewBag.user_id = new SelectList(db.users, "user_id", "user_name");
            return View();
        }

        // POST: passbooks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SavingsBookID,user_id,OpeningDate,InitialDepositAmount,DepositAmount,InterestRate,SavingsType,IsClosed")] passbook passbook)
        {
            //if (passbook.DepositAmount > passbook.InitialDepositAmount)
            //{
            //    ModelState.AddModelError("DepositAmount", "Deposit amount cannot be larger than the initial deposit amount.");
            //    return View(passbook); // Return the view with the error
            //}

            if (ModelState.IsValid)
            {
                            passbook.IsClosed = false;


                db.passbooks.Add(passbook);
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            ViewBag.SavingsType = new SelectList(db.SavingsAccountTypes, "SavingsTypeID", "AccountTypeName", passbook.SavingsType);
            ViewBag.user_id = new SelectList(db.users, "user_id", "user_name", passbook.user_id);
            return View(passbook);
        }

        // GET: passbooks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            passbook passbook = db.passbooks.Find(id);
            if (passbook == null)
            {
                return HttpNotFound();
            }
            ViewBag.SavingsType = new SelectList(db.SavingsAccountTypes, "SavingsTypeID", "AccountTypeName", passbook.SavingsType);
            ViewBag.user_id = new SelectList(db.users, "user_id", "user_address", passbook.user_id);
            return View(passbook);
        }

        // POST: passbooks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SavingsBookID,user_id,OpeningDate,InitialDepositAmount,DepositAmount,InterestRate,SavingsType,IsClosed")] passbook passbook)
        {
            if (ModelState.IsValid)
            {
                db.Entry(passbook).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SavingsType = new SelectList(db.SavingsAccountTypes, "SavingsTypeID", "AccountTypeName", passbook.SavingsType);
            ViewBag.user_id = new SelectList(db.users, "user_id", "user_address", passbook.user_id);
            return View(passbook);
        }

        // GET: passbooks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            passbook passbook = db.passbooks.Find(id);
            if (passbook == null)
            {
                return HttpNotFound();
            }
            return View(passbook);
        }

        // POST: passbooks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            passbook passbook = db.passbooks.Find(id);
            db.passbooks.Remove(passbook);
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
