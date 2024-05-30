using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace AccountingSystem.Models
{
    public class Supplier
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        
    }
}
