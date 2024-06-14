using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
namespace AccountingSystem.Models
{
    public class Item
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("UserId")]
        [Required]
        public string UserId { get; set; }

        [BsonElement("Barcode")]
        [BsonRequired]
        public string Barcode { get; set; }

        [BsonElement("Name")]
        [Display(Name = "Назва товару")]
        [Required]
        public string Name { get; set; }

        [BsonElement("Category")]
        [Required]
        public string Category { get; set; }

        [BsonElement("Available")]
        [Required]
        [Display(Name = "Доступно")]
        public double Available { get; set; }

        [BsonElement("Price")]
        [Display(Name = "Ціна(грн)")]
        [Required]
        public double Price { get; set; }

        [BsonElement("PurchasePrice")]
        [Display(Name = "Собівартість(грн)")]
        [Required]
        public double PurchPrice { get; set; }

        [BsonElement("MarkupPriceInterest")]
        public double MarkupPriceInterest { get; set; }

        [BsonElement("MarkupPriceNumeric")]
        public double MarkupPriceNumeric { get; set; }

        [BsonElement("ImageUrl")]
        [Display(Name = "Photo")]
        [DataType(DataType.ImageUrl)]
        [Required]
        public string ImageUrl { get; set; }

        [BsonElement("Unit")]
        [Required]
        public string Unit { get; set; }

        [BsonElement("SupplierId")]
        [Required]
        public string SupplierId { get; set; }

        public double Discount { get; set; }
        public double DiscountedPrice { get; set; }
    }
}