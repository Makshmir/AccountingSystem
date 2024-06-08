using MongoDB.Driver;
using AccountingSystem.Models;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Configuration;

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

        public List<Check> GetByUserId(string userId)
        {
            return checks.Find(check => check.UserId == userId).ToList();
        }

        public Check Get(string id, string userId)
        {
            return checks.Find(check => check.Id == id && check.UserId == userId).FirstOrDefault();
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
                        throw new Exception($"Запитувана кількість товару \"{item.Name}\" перевищує доступну кількість на складі.");
                    }
                }
            }

            var check = new Check
            {
                UserId = model.UserId,
                Items = itemsList,
                Sum = Math.Round(totalSum, 2),
                Date = DateTime.Now,
                Profit = Math.Round(totalProfit, 2)
            };

            checks.InsertOne(check);
            return check;
        }

        public CheckDetailsViewModel GetCheckDetails(string id, string userId)
        {
            var check = checks.Find(c => c.Id == id && c.UserId == userId).FirstOrDefault();
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

        public void Delete(string id, string userId)
        {
            checks.DeleteOne(check => check.Id == id && check.UserId == userId);
        }

        public void ReturnItemsToStock(List<OrderItemViewModel> itemsList)
        {
            foreach (var orderItem in itemsList)
            {
                Item item = items.Find(i => i.Id == orderItem.Id).FirstOrDefault();
                if (item != null)
                {
                    item.Available += orderItem.Quantity;
                    var filter = Builders<Item>.Filter.Eq("Id", item.Id);
                    var update = Builders<Item>.Update.Set("Available", item.Available);
                    items.UpdateOne(filter, update);
                }
            }
        }
    }
}
