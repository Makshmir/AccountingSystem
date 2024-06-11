namespace AccountingSystem.Models
{
    public class SupplyViewModel
    {
        public string Id { get; set; }
        public string SupplierName { get; set; }
        public DateTime SupplyDate { get; set; }
        public double TotalAmount { get; set; }
        public List<SupplyItemViewModel> Items { get; set; }
    }

    public class SupplyItemViewModel
    {
        public string ItemName { get; set; }
        public double Quantity { get; set; }
        public double PurchasePrice { get; set; }
        public double TotalPrice { get; set; }
    }

}
