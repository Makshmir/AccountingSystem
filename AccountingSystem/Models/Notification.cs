using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace AccountingSystem.Models
{
    public class Notification
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public List<string> Categories { get; set; }
        public string Message { get; set; }
    }
}
