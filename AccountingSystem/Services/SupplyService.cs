using MongoDB.Driver;
using AccountingSystem.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace AccountingSystem.Services
{
    public class SupplyService
    {
        private readonly IMongoCollection<Supply> supplies;

        public SupplyService(IConfiguration config)
        {
            MongoClient client = new MongoClient(config.GetConnectionString("AccountingDb"));
            IMongoDatabase database = client.GetDatabase("AccountingDb");
            supplies = database.GetCollection<Supply>("Supplies");
        }

        public List<Supply> GetByUserId(string userId)
        {
            return supplies.Find(supply => supply.UserId == userId).ToList();
        }

        public Supply GetByUserId(string id, string userId)
        {
            return supplies.Find(supply => supply.Id == id && supply.UserId == userId).FirstOrDefault();
        }

        public Supply Create(Supply supply)
        {
            supplies.InsertOne(supply);
            return supply;
        }

        public void Update(string id, Supply supplyIn)
        {
            supplies.ReplaceOne(supply => supply.Id == id && supply.UserId == supplyIn.UserId, supplyIn);
        }

        public void Remove(string id, string userId)
        {
            supplies.DeleteOne(supply => supply.Id == id && supply.UserId == userId);
        }
    }
}
