using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace AccountingSystem.Models
{
    public class Supply
    {
        [BsonElement("UserId")]
        [Required]
        public string UserId { get; set; }
        public string Id { get; set; }
        public string SupplierId { get; set; }
        [DataType(DataType.Date)]
        public DateTime SupplyDate { get; set; }
        public List<SupplyItem> Items { get; set; }
        public double TotalAmount { get; set; }
    }

    public class SupplyItem
    {
        [Required]
        public string ItemId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Кількість повинна бути більше 0")]
        public double Quantity { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Ціна повинна бути більше 0")]
        public double PurchasePrice { get; set; }
        public double TotalPrice => Math.Round(Quantity * PurchasePrice, 2);
    }
}
