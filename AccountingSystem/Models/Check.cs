using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace AccountingSystem.Models
{
    public class Check
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("UserId")]
        [Required]
        public string UserId { get; set; }

        [BsonElement("Date")]
        public DateTime Date { get; set; }

        [BsonElement("Sum")]
        public double Sum {  get; set; }

        [BsonElement("Profit")]
        public double Profit { get; set; }

        [BsonElement("Items")]
        [Display(Name = "Список товарів")]
        public List<OrderItemViewModel> Items { get; set; }

    }
}
