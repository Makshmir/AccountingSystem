using System;
using System.Collections.Generic;
using System.Linq;
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

                    if (orderItem.Quantity <= item.Available)
                    {
                        double itemTotal = Math.Round(orderItem.Quantity * item.Price, 2);
                        totalSum += itemTotal;

                        double itemTotalProfit = Math.Round(orderItem.Quantity * item.MarkupPriceNumeric, 2);
                        totalProfit += itemTotalProfit;

                        item.Available -= Math.Round(orderItem.Quantity, 2);
                        var filter = Builders<Item>.Filter.Eq("Id", item.Id);
                        var update = Builders<Item>.Update.Set("Available", item.Available);
                        items.UpdateOne(filter, update);
                    }
                    else
                    {
                        // Обробка ситуації, коли запитувана кількість перевищує доступну
                        throw new Exception($"Запитувана кількість товару \"{item.Name}\" перевищує доступну кількість на складі.");
                    }
                }
            }

            var check = new Check
            {
                Items = itemsList,
                Sum = Math.Round(totalSum, 2),
                Date = DateTime.Now,
                Profit = Math.Round(totalProfit, 2)
            };

            checks.InsertOne(check);
            return check;
        }

        public CheckDetailsViewModel GetCheckDetails(string id)
        {
            var check = checks.Find(c => c.Id == id).FirstOrDefault();
            if (check == null)
            {
                return null;
            }

            var itemsList = new List<Item>();
            foreach (var orderItem in check.Items)
            {
                var item = items.Find(i => i.Id == orderItem.Id).FirstOrDefault();
                if (item != null)
                {
                    itemsList.Add(item);
                }
            }

            return new CheckDetailsViewModel
            {
                Check = check,
                Items = itemsList
            };
        }


        public void Delete(string id)
        {
            checks.DeleteOne(check => check.Id == id);
        }

        public void ReturnItemsToStock(List<OrderItemViewModel> itemsList)
        {
            foreach (var orderItem in itemsList)
            {
                Item item = items.Find(i => i.Id == orderItem.Id).FirstOrDefault();
                if (item != null)
                {
                    // Додавання кількості товару до доступного залишку на складі
                    item.Available += orderItem.Quantity;
                    var filter = Builders<Item>.Filter.Eq("Id", item.Id);
                    var update = Builders<Item>.Update.Set("Available", item.Available);
                    items.UpdateOne(filter, update);
                }
            }
        }
    }
}
