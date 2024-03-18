using System;
using System.Collections.Generic;

namespace TimeShareProject.Models
{
    public partial class DashboardData
    {
        public List<int> Chart { get; set; }
        public List<int> MonthCount {  get; set; }
        public int Count {  get; set; }
    }
}
