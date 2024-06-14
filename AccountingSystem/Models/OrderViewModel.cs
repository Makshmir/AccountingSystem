using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace AccountingSystem.Models
{
    public class OrderViewModel
    {

        [BsonElement("UserId")]
        [Required]
        public string UserId { get; set; }
        public List<OrderItemViewModel> Items { get; set; }
    }

    public class OrderItemViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; } 
        public double TotalPrice => Price * Quantity;
        public string Category { get; set; }
        public double PurchasePrice { get; set; }
    }


}
