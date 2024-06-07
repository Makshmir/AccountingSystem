using AccountingSystem.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace AccountingSystem.Services
{
    public class SupplyService
    {
        private readonly IMongoCollection<Supply> _supplies;

        public SupplyService(IConfiguration config)
        {
            //var client = new MongoClient(config.ConnectionString);
            //var database = client.GetDatabase(config.DatabaseName);
            //_supplies = database.GetCollection<Supply>(config.SuppliesCollectionName);

            MongoClient client = new MongoClient(config.GetConnectionString("AccountingDb"));
            IMongoDatabase database = client.GetDatabase("AccountingDb");
            _supplies = database.GetCollection<Supply>("Supplies");


        }

        public List<Supply> Get() => _supplies.Find(supply => true).ToList();

        public Supply Get(string id) => _supplies.Find<Supply>(supply => supply.Id == id).FirstOrDefault();

        public Supply Create(Supply supply)
        {
            supply.TotalAmount = supply.Items.Sum(item => item.TotalPrice);
            _supplies.InsertOne(supply);
            return supply;
        }

        public void Update(string id, Supply supplyIn)
        {
            supplyIn.TotalAmount = supplyIn.Items.Sum(item => item.TotalPrice);
            _supplies.ReplaceOne(supply => supply.Id == id, supplyIn);
        }

        public void Remove(string id) => _supplies.DeleteOne(supply => supply.Id == id);
    }
}
