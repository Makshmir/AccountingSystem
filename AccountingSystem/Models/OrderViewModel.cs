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

        [Range(0.1, Double.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public double Quantity { get; set; }
    }

}
