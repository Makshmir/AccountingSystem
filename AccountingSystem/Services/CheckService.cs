using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using AccountingSystem.Models;

namespace AccountingSystem.Services
{
    public class CheckService
    {
        private readonly IMongoCollection<Check> checks;
        private readonly IMongoCollection<Item> items;

        public CheckService(IConfiguration config)
        {
            MongoClient client = new MongoClient(config.GetConnectionString("AccountingDb"));
            IMongoDatabase database = client.GetDatabase("AccountingDb");
            checks = database.GetCollection<Check>("Checks");
            items = database.GetCollection<Item>("items");
        }


        public List<Check> Get()
        {
            return checks.Find(item => true).ToList();
        }

        public Check Get(string id)
        {
            return checks.Find(item => item.Id == id).FirstOrDefault();
        }


        public Check Create(OrderViewModel model)
        {

            List<OrderItemViewModel> itemsList = model.Items;
            double totalSum = 0;
            double totalProfit = 0;
            foreach (var orderItem in itemsList)
            {
                Item item = items.Find(i => i.Id == orderItem.Id).FirstOrDefault();
                if (item != null)
                {
                    double itemTotal = orderItem.Quantity * item.Price;
                    totalSum += itemTotal;

                    double itemTotalProfit = orderItem.Quantity * item.MarkupPriceNumeric;
                    totalProfit += itemTotalProfit;

                }
            }






            var check = new Check
            {
                Items = itemsList,
                Sum = totalSum,
                Date=DateTime.Now,
                Profit=totalProfit
            };

            checks.InsertOne(check);
            //double sum = 0;
            //foreach (var item in itemsList)
            //{
            //    var product = items.Find(product => product.Id == item.Id);
            //    doubleproduct.Price;
            //    sum += item.Quantity
            //}
            //checks.InsertOne(check);
            return check;
        }
    }
}
