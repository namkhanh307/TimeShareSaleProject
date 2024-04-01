using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TimeShareProject.Models;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System;



namespace TimeShareProject.Controllers
{
    public class DashboardController : Controller
    {
        private readonly _4restContext _dbContext;
        public DashboardController(_4restContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        private DashboardInfo GetDashboardData()
        {
            DashboardInfo dashboardInfo = new DashboardInfo();
            List<Transaction> transactions = new List<Transaction>();
            List<int> revenue = new List<int>(12) { };
            List<String> months = new List<String>();
            var successTransaction = _dbContext.Transactions.Where(t => t.Status == true).Count();
            var totalTransaction = _dbContext.Transactions.Count();
            var members = _dbContext.Accounts.Where(m => m.Role == 3).Count();
            var properties = _dbContext.Properties.Count();
            var blocks = _dbContext.Properties.Count() * 52;
            var undone = (int)totalTransaction - (int)successTransaction;
            var projects = _dbContext.Projects.Count();
            var progress1 = GetProjectProgress(1);
            var progress2 = GetProjectProgress(2);
            var progress3 = GetProjectProgress(3);

            for (int i = 0; i < revenue.Count(); i++)
            {
                revenue.Add(0);
            }

            transactions = _dbContext.Set<Transaction>().Where(t => t.Status == true).ToList();
            var results = from line in transactions
                          where line.Date.HasValue // Check that Date is not null
                          group line by line.Date.Value.Month into g // Access the Month value safely
                          select new
                          {
                              Month = g.Key,
                              Price = g.Sum(pc => pc.Amount ?? 0).ToString(), // Use ?? operator to handle nullable Amount
                          };

            results = results.OrderBy(p => p.Month); // Correctly order the results by Month
            foreach (var item in results)
            {
                revenue.Add(int.Parse(item.Price));
                months.Add(item.Month.ToString());
            }
            dashboardInfo.Months = months;
            dashboardInfo.Amount = revenue;
            dashboardInfo.SuccessTransaction = successTransaction;
            dashboardInfo.TotalTransaction = totalTransaction;
            dashboardInfo.TotalProject = projects;
            dashboardInfo.TotalProperty = properties;
            dashboardInfo.TotalBlock = blocks;
            dashboardInfo.TotalMember = members;
            dashboardInfo.ProgressProject1 = progress1;
            dashboardInfo.ProgressProject2 = progress2;
            dashboardInfo.ProgressProject3 = progress3;
            dashboardInfo.UndoneTransaction = undone;

            return dashboardInfo;

        }

        public int GetProjectProgress(int projectId)
        {
            int progress = 0;
            List<Reservation> reservationList = new List<Reservation>();
            List<Property> propertyIdList = new List<Property>();
            propertyIdList = _dbContext.Properties.Where(p => p.ProjectId == projectId).ToList();
            reservationList = _dbContext.Reservations.ToList();
            foreach (var item in propertyIdList)
            {
                int key = item.Id;
                foreach (var row in reservationList)
                {
                    if (row.PropertyId == key)
                    {
                        progress = progress + 1;
                    }
                }
            }
            return progress;
        }

        public IActionResult Index()
        {
            var data = GetDashboardData();
            String jsonData = JsonConvert.SerializeObject(data.Amount);
            String jsonData2 = JsonConvert.SerializeObject(data.Months);
            ViewData["DoubleListJson"] = jsonData;
            ViewData["Months"] = jsonData2;
            ViewBag.data = data;
            return View();
        }
    }
}
