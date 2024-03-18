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
        private List<int> GetDashboardData()
        {
            DashboardData dashboardData = new DashboardData();
            List<Transaction> transactions = new List<Transaction>();
            List<int> revenue= new List<int>(12) {};
            for(int i = 0; i < revenue.Count(); i++)
            {
                revenue.Add(0);
            }
            transactions = _dbContext.Set<Transaction>().Where(t=>t.Status==true).ToList();
            //var results = from line in transactions
            //              group line by line.Date.Month into g
            //              select new 
            //              {
            //                  Month = g.Key,
            //                  Price = g.Sum(pc => pc.Amount).ToString(),
            //              };
            //results.OrderBy(p => p.Month);
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
            }
            //foreach (Transaction t in transactions)
            //{
            //    double amount = 0;
            //    int month = t.Date.Month - 1;
            //    if (t.Amount != null)
            //    {
            //        revenue.Add((int)t.Amount);

            //    }

            //}
            
            return revenue;
        }
        public IActionResult Index()
        {
            var revenue = GetDashboardData();
            String jsonData= JsonConvert.SerializeObject(revenue);
            ViewData["DoubleListJson"]=jsonData;
            return View();
        }
    }
}
