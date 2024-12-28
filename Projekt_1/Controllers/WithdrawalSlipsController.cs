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
    public class WithdrawalSlipsController : Controller
    {
        private Project1DBEntities db = new Project1DBEntities();

        // GET: WithdrawalSlips
        public ActionResult Index(string sortOrder, string searchString)
        {
            ViewBag.SavingsBookIDSortParam = String.IsNullOrEmpty(sortOrder) ? "savingsBookID_desc" : "";
            ViewBag.DateSortParam = sortOrder == "Date" ? "date_desc" : "Date";

            var withdrawalSlips = from w in db.WithdrawalSlips
                                  select w;

            if (!String.IsNullOrEmpty(searchString))
            {
                withdrawalSlips = withdrawalSlips.Where(w => w.passbook.SavingsBookID.ToString().Contains(searchString));
            }

            switch (sortOrder)
            {
                case "savingsBookID_desc":
                    withdrawalSlips = withdrawalSlips.OrderByDescending(w => w.SavingsBookID);
                    break;
                case "Date":
                    withdrawalSlips = withdrawalSlips.OrderBy(w => w.WithdrawalDate);
                    break;
                case "date_desc":
                    withdrawalSlips = withdrawalSlips.OrderByDescending(w => w.WithdrawalDate);
                    break;
                default:
                    withdrawalSlips = withdrawalSlips.OrderBy(w => w.SavingsBookID);
                    break;
            }

            return View(withdrawalSlips.ToList());
        }


        // GET: WithdrawalSlips/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WithdrawalSlip withdrawalSlip = db.WithdrawalSlips.Find(id);
            if (withdrawalSlip == null)
            {
                return HttpNotFound();
            }
            return View(withdrawalSlip);
        }

        // GET: WithdrawalSlips/Create
        public ActionResult Create()
        {
            // Chỉ chọn SavingsBook có chỉ số IsClosed là false
            ViewBag.SavingsBookID = new SelectList(db.passbooks.Where(p => p.IsClosed != true), "SavingsBookID", "SavingsBookID");
            ViewBag.user_id = new SelectList(db.users, "user_id", "user_name");
            return View(new WithdrawalSlip());
        }




        [HttpGet]
        public JsonResult GetCustomerDetails(int savingsBookID)
        {
            var customerDetails = db.passbooks
                                    .Where(p => p.SavingsBookID == savingsBookID)
                                    .Select(p => new { user_id = p.user_id, user_name = p.user.user_name })
                                    .FirstOrDefault();
            return Json(customerDetails, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDepositAmount(int savingsBookID)
        {
            var depositAmount = db.passbooks
                                  .Where(p => p.SavingsBookID == savingsBookID)
                                  .Select(p => p.DepositAmount)
                                  .FirstOrDefault();
            return Json(depositAmount, JsonRequestBehavior.AllowGet);
        }


        // POST: WithdrawalSlips/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "WithdrawalID,SavingsBookID,user_id,WithdrawalAmount,WithdrawalDate")] WithdrawalSlip withdrawalSlip)
        {
            var passbook = db.passbooks
                             .Where(p => p.SavingsBookID == withdrawalSlip.SavingsBookID)
                             .FirstOrDefault();

            if (passbook != null)
            {
                var savingsAccountType = db.SavingsAccountTypes
                                           .Where(s => s.SavingsTypeID == passbook.SavingsType)
                                           .FirstOrDefault();

                if (savingsAccountType != null)
                {
                    // Kiểm tra điều kiện loại tiết kiệm
                    if (savingsAccountType.SavingsTypeID == 1) // Không kỳ hạn
                    {
                        var daysDiff = (DateTime.Now - passbook.OpeningDate.GetValueOrDefault()).TotalDays;

                        if (daysDiff <= 15)
                        {
                            ModelState.AddModelError("", "Loại tiết kiệm không kỳ hạn chỉ được rút sau 15 ngày.");
                            InitializeViewBag(withdrawalSlip);
                            return View(withdrawalSlip);
                        }

                        if (withdrawalSlip.WithdrawalAmount > passbook.DepositAmount)
                        {
                            ModelState.AddModelError("", "Số tiền rút không được vượt quá số tiền hiện có.");
                            InitializeViewBag(withdrawalSlip);
                            return View(withdrawalSlip);
                        }
                    }
                    else if (savingsAccountType.SavingsTypeID == 2 || savingsAccountType.SavingsTypeID == 3)
                    {
                        // Có kỳ hạn
                        var monthsDiff = ((DateTime.Now - passbook.OpeningDate.GetValueOrDefault()).TotalDays) / 30;

                        if (monthsDiff < savingsAccountType.Term)
                        {
                            ModelState.AddModelError("", "Loại tiết kiệm có kỳ hạn chỉ được rút khi hết kỳ hạn.");
                            InitializeViewBag(withdrawalSlip);
                            return View(withdrawalSlip);
                        }

                        if (withdrawalSlip.WithdrawalAmount != passbook.DepositAmount)
                        {
                            ModelState.AddModelError("", "Phải rút toàn bộ số tiền trong tài khoản.");
                            InitializeViewBag(withdrawalSlip);
                            return View(withdrawalSlip);
                        }
                    }
                }

                passbook.DepositAmount -= withdrawalSlip.WithdrawalAmount;
                withdrawalSlip.user_id = passbook.user_id;

                if (passbook.DepositAmount == 0)
                {
                    passbook.IsClosed = true;
                }

                if (ModelState.IsValid)
                {
                    db.WithdrawalSlips.Add(withdrawalSlip);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            InitializeViewBag(withdrawalSlip);
            return View(withdrawalSlip);
        }

        private void InitializeViewBag(WithdrawalSlip withdrawalSlip)
        {
            ViewBag.SavingsBookID = new SelectList(db.passbooks, "SavingsBookID", "SavingsBookID", withdrawalSlip.SavingsBookID);
            ViewBag.user_id = new SelectList(db.users, "user_id", "user_name", withdrawalSlip.user_id);
        }




        // GET: WithdrawalSlips/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WithdrawalSlip withdrawalSlip = db.WithdrawalSlips.Find(id);
            if (withdrawalSlip == null)
            {
                return HttpNotFound();
            }
            ViewBag.SavingsBookID = new SelectList(db.passbooks, "SavingsBookID", "SavingsBookID", withdrawalSlip.SavingsBookID);
            ViewBag.user_id = new SelectList(db.users, "user_id", "user_address", withdrawalSlip.user_id);
            return View(withdrawalSlip);
        }

        // POST: WithdrawalSlips/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "WithdrawalID,SavingsBookID,user_id,WithdrawalAmount,WithdrawalDate")] WithdrawalSlip withdrawalSlip)
        {
            if (ModelState.IsValid)
            {
                db.Entry(withdrawalSlip).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SavingsBookID = new SelectList(db.passbooks, "SavingsBookID", "SavingsBookID", withdrawalSlip.SavingsBookID);
            ViewBag.user_id = new SelectList(db.users, "user_id", "user_address", withdrawalSlip.user_id);
            return View(withdrawalSlip);
        }

        // GET: WithdrawalSlips/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WithdrawalSlip withdrawalSlip = db.WithdrawalSlips.Find(id);
            if (withdrawalSlip == null)
            {
                return HttpNotFound();
            }
            return View(withdrawalSlip);
        }

        // POST: WithdrawalSlips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WithdrawalSlip withdrawalSlip = db.WithdrawalSlips.Find(id);
            db.WithdrawalSlips.Remove(withdrawalSlip);
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
