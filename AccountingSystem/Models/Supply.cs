using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AccountingSystem.CustomAttributes;
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
        public DateTime SupplyDate { get; set; }
        public List<SupplyItem> Items { get; set; }
        public double TotalAmount { get; set; }
    }

    public class SupplyItem
    {
        public string ItemId { get; set; }
        public int Quantity { get; set; }
        public double PurchasePrice { get; set; }
        public double TotalPrice => Quantity * PurchasePrice;
    }



}
