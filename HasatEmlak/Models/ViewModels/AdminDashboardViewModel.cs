namespace HasatEmlak.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalProperties { get; set; }
        public int ActiveProperties { get; set; }
        public int TotalAgents { get; set; }
        public int TotalUsers { get; set; }
        public int UnreadMessages { get; set; }
        public int PropertiesThisMonth { get; set; }

        public decimal TotalSalesValue { get; set; }
        public decimal AveragePropertyPrice { get; set; }

        public List<MonthlyStats> MonthlyStats { get; set; } = new List<MonthlyStats>();
        public List<CategoryStats> CategoryStats { get; set; } = new List<CategoryStats>();
        public List<CityStats> CityStats { get; set; } = new List<CityStats>();
    }

    public class MonthlyStats
    {
        public string Month { get; set; }
        public int PropertyCount { get; set; }
        public decimal TotalValue { get; set; }
    }

    public class CategoryStats
    {
        public string CategoryName { get; set; }
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    public class CityStats
    {
        public string CityName { get; set; }
        public int Count { get; set; }
    }
}
