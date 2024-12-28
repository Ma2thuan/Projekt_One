using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Projekt_1.Model;

namespace Projekt_1.Controllers
{
    public class ReportsController : Controller
    {
        private Project1DBEntities db = new Project1DBEntities();

        // Định nghĩa lớp ReportData
        public class ReportData
        {
            public string SavingsType { get; set; }
            public int? TotalIncome { get; set; }
            public int? TotalExpense { get; set; }
            public int? Difference { get; set; }
            public DateTime? DepositDate { get; set; } // Thêm thuộc tính DepositDate
        }

        // GET: Reports/MonthlyFilter
        public ActionResult MonthlyFilter()
        {
            ViewBag.SavingsTypes = new SelectList(db.passbooks.Select(p => p.SavingsType.ToString()).Distinct().ToList());
            return View();
        }

        // GET: Reports/MonthlyReport
        public ActionResult MonthlyReport(DateTime? reportMonth, string savingsType)
        {
            // Kiểm tra điều kiện tháng và loại tiết kiệm
            if (reportMonth.HasValue && !string.IsNullOrEmpty(savingsType))
            {
                var depositGroups = db.SavingsDeposits
                    .Where(d => d.DepositDate.Value.Year == reportMonth.Value.Year && d.DepositDate.Value.Month == reportMonth.Value.Month)
                    .Join(
                        db.passbooks,
                        d => d.SavingsBookID,
                        p => p.SavingsBookID,
                        (d, p) => new { Amount = d.DepositAmount ?? 0, SavingsType = p.SavingsType.ToString(), d.DepositDate, d.SavingsBookID }
                    )
                    .Where(dp => dp.SavingsType == savingsType)
                    .GroupBy(dp => new { dp.SavingsType, dp.DepositDate })
                    .Select(g => new { g.Key.SavingsType, g.Key.DepositDate, TotalIncome = g.Sum(dp => dp.Amount) })
                    .ToList();

                var withdrawalGroups = db.WithdrawalSlips
                    .Where(w => w.WithdrawalDate.Value.Year == reportMonth.Value.Year && w.WithdrawalDate.Value.Month == reportMonth.Value.Month)
                    .Join(
                        db.passbooks,
                        w => w.SavingsBookID,
                        p => p.SavingsBookID,
                        (w, p) => new { Amount = w.WithdrawalAmount ?? 0, SavingsType = p.SavingsType.ToString(), w.WithdrawalDate, w.SavingsBookID }
                    )
                    .Where(wp => wp.SavingsType == savingsType)
                    .GroupBy(wp => new { wp.SavingsType, wp.WithdrawalDate })
                    .Select(g => new { g.Key.SavingsType, g.Key.WithdrawalDate, TotalExpense = g.Sum(wp => wp.Amount) })
                    .ToList();

                var reportData = depositGroups
                    .GroupJoin(withdrawalGroups, dg => new { dg.SavingsType, Date = dg.DepositDate }, wg => new { wg.SavingsType, Date = wg.WithdrawalDate }, (dg, wg) => new { dg.SavingsType, dg.DepositDate, Deposits = dg, Withdrawals = wg.FirstOrDefault() })
                    .Select(r => new ReportData
                    {
                        SavingsType = r.SavingsType,
                        DepositDate = r.DepositDate,
                        TotalIncome = r.Deposits?.TotalIncome ?? 0,
                        TotalExpense = r.Withdrawals?.TotalExpense ?? 0,
                        Difference = (r.Deposits?.TotalIncome ?? 0) - (r.Withdrawals?.TotalExpense ?? 0)
                    }).ToList();

                ViewBag.ReportData = reportData;
                ViewBag.ReportMonth = reportMonth.Value.ToString("MM/yyyy");
                ViewBag.SelectedSavingsType = savingsType;
            }

            return View();
        }
    }
}
