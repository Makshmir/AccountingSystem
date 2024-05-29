namespace AccountingSystem.Models
{
    public class OrderViewModel
    {
        public List<OrderItemViewModel> Items { get; set; }
    }

    public class OrderItemViewModel
    {
        public string Id { get; set; }
        public int Quantity { get; set; }
    }

}
