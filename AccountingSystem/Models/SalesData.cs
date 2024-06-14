namespace AccountingSystem.Models
{
    public class SalesData
    {
        public string Category { get; set; }
        public float TotalSales { get; set; }
        public DateTime Date { get; set; } 
    }

    public class SalesPrediction
    {
        public float Score { get; set; }
    }
}
