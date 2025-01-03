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
    public class SavingsAccountTypesController : Controller
    {
        private Project1DBEntities db = new Project1DBEntities();

        // GET: SavingsAccountTypes
        public ActionResult Index()
        {
            return View(db.SavingsAccountTypes.ToList());
        }

        // GET: SavingsAccountTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SavingsAccountType savingsAccountType = db.SavingsAccountTypes.Find(id);
            if (savingsAccountType == null)
            {
                return HttpNotFound();
            }
            return View(savingsAccountType);
        }

        // GET: SavingsAccountTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SavingsAccountTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SavingsTypeID,AccountTypeName,InterestRate,Term,WithdrawalDays,MinimumDeposit,IsActive,AllowsAdditionalDeposits")] SavingsAccountType savingsAccountType)
        {
            if (ModelState.IsValid)
            {
                db.SavingsAccountTypes.Add(savingsAccountType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(savingsAccountType);
        }

        // GET: SavingsAccountTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SavingsAccountType savingsAccountType = db.SavingsAccountTypes.Find(id);
            if (savingsAccountType == null)
            {
                return HttpNotFound();
            }
            return View(savingsAccountType);
        }

        // POST: SavingsAccountTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SavingsTypeID,AccountTypeName,InterestRate,Term,WithdrawalDays,MinimumDeposit,IsActive,AllowsAdditionalDeposits")] SavingsAccountType savingsAccountType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(savingsAccountType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(savingsAccountType);
        }

        // GET: SavingsAccountTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SavingsAccountType savingsAccountType = db.SavingsAccountTypes.Find(id);
            if (savingsAccountType == null)
            {
                return HttpNotFound();
            }
            return View(savingsAccountType);
        }

        // POST: SavingsAccountTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SavingsAccountType savingsAccountType = db.SavingsAccountTypes.Find(id);
            db.SavingsAccountTypes.Remove(savingsAccountType);
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
