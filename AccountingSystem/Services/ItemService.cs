using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using AccountingSystem.Models;

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

        public List<Item> Get()
        {
            return items.Find(item => true).ToList();
        }

        public Item Get(string id)
        {
            return items.Find(item => item.Id == id).FirstOrDefault();
        }

        public Item Create(Item item)
        {
            items.InsertOne(item);
            return item;
        }

        public void Update(string id, Item itemIn)
        {
            itemIn.MarkupPriceNumeric = Math.Round(itemIn.Price - itemIn.PurchPrice, 2);
            itemIn.MarkupPriceInterest = Math.Round(itemIn.Price / itemIn.PurchPrice-1, 2);
            items.ReplaceOne(item => item.Id == id, itemIn);
        }

        public void Remove(Item itemIn)
        {
            items.DeleteOne(item => item.Id == itemIn.Id);
        }

        public void Remove(string id)
        {
            items.DeleteOne(item => item.Id == id);
        }
    }
}
