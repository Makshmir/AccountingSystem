using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using AccountingSystem.Models;
using System;

namespace AccountingSystem.Services
{
    public class ItemService
    {
        private readonly IMongoCollection<Item> items;

        public ItemService(IConfiguration config)
        {
            MongoClient client = new MongoClient(config.GetConnectionString("AccountingDb"));
            IMongoDatabase database = client.GetDatabase("AccountingDb");
            items = database.GetCollection<Item>("items");
        }

        public List<Item> GetByUserId(string userId)
        {
            return items.Find(item => item.UserId == userId).ToList();
        }

        public Item Get(string id, string userId)
        {
            return items.Find(item => item.Id == id && item.UserId == userId).FirstOrDefault();
        }

        public Item Create(Item item)
        {
            items.InsertOne(item);
            return item;
        }

        public void Update(string id, Item itemIn, string userId)
        {
            itemIn.MarkupPriceNumeric = Math.Round(itemIn.Price - itemIn.PurchPrice, 2);
            itemIn.MarkupPriceInterest = Math.Round((itemIn.Price / itemIn.PurchPrice - 1) * 100, 2);
            items.ReplaceOne(item => item.Id == id && item.UserId == userId, itemIn);
        }

        public void UpdateQuantity(string itemId, double quantity, string userId)
        {
            var item = items.Find<Item>(i => i.Id == itemId && i.UserId == userId).FirstOrDefault();
            if (item != null)
            {
                item.Available += quantity;
                items.ReplaceOne(i => i.Id == itemId && i.UserId == userId, item);
            }
        }

        public void UpdateQuantityAndPurchasePrice(string itemId, double quantity, double purchasePrice, string userId)
        {
            var item = items.Find<Item>(i => i.Id == itemId && i.UserId == userId).FirstOrDefault();
            if (item != null)
            {
                item.Available += quantity;
                item.PurchPrice = purchasePrice;
                items.ReplaceOne(i => i.Id == itemId && i.UserId == userId, item);
            }
        }

        public void Remove(Item itemIn, string userId)
        {
            items.DeleteOne(item => item.Id == itemIn.Id && item.UserId == userId);
        }

        public void Remove(string id, string userId)
        {
            items.DeleteOne(item => item.Id == id && item.UserId == userId);
        }
    }
}
