using MongoDB.Driver;
using AccountingSystem.Models;

namespace AccountingSystem.Services
{
    public class SupplierService
    {
        private readonly IMongoCollection<Supplier> suppliers;
        public SupplierService(IConfiguration config)
        {
            MongoClient client = new MongoClient(config.GetConnectionString("AccountingDb"));
            IMongoDatabase database = client.GetDatabase("AccountingDb");
            suppliers = database.GetCollection<Supplier>("Suppliers");
        }

        public List<Supplier> GetByUserId(string userId)
        {
            return suppliers.Find(supplier => supplier.UserId == userId).ToList();
        }

        public Supplier Get(string id, string userId)
        {
            return suppliers.Find(supplier => supplier.Id == id && supplier.UserId == userId).FirstOrDefault();
        }

        public Supplier Create(Supplier supplier)
        {
            suppliers.InsertOne(supplier);
            return supplier;
        }

        public void Remove(string id, string userId)
        {
            suppliers.DeleteOne(supplier => supplier.Id == id && supplier.UserId == userId);
        }
    }
}
