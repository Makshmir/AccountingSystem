namespace AccountingSystem.Models
{
    public class CheckDetailsViewModel
    {
        public Check Check { get; set; }
        public List<OrderItemViewModel> Items { get; set; }
    }

}
