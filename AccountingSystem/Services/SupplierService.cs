using AccountingSystem.Models;
using MongoDB.Driver;

namespace AccountingSystem.Services
{
    public class SupplierService
    {

        private readonly IMongoCollection<Check> checks;
        private readonly IMongoCollection<Item> items;
        private readonly IMongoCollection<Supplier> suppliers;

        public SupplierService(IConfiguration config)
        {
            MongoClient client = new MongoClient(config.GetConnectionString("AccountingDb"));
            IMongoDatabase database = client.GetDatabase("AccountingDb");
            checks = database.GetCollection<Check>("Checks");
            items = database.GetCollection<Item>("items");
            suppliers= database.GetCollection<Supplier>("suppliers");
        }



        public List<Supplier> Get()
        {
            return suppliers.Find(item => true).ToList();
        }

        public Supplier Get(string id)
        {
            return suppliers.Find(item => item.Id == id).FirstOrDefault();
        }


        public Supplier Create(Supplier supplier)
        {
            suppliers.InsertOne(supplier);
            return supplier;
        }



        public void Remove(Supplier suppIn)
        {
            suppliers.DeleteOne(item => item.Id == suppIn.Id);
        }

        public void Remove(string id)
        {
            suppliers.DeleteOne(item => item.Id == id);
        }

    }
}
