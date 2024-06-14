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

        public List<SalesData> GetSalesByCategoryDaily(string userId, DateTime? startDate, DateTime? endDate)
        {
            var filterBuilder = Builders<Check>.Filter;
            var filter = filterBuilder.Eq(c => c.UserId, userId);

            if (startDate.HasValue)
            {
                filter &= filterBuilder.Gte(c => c.Date, startDate.Value);
            }
            if (endDate.HasValue)
            {
                filter &= filterBuilder.Lte(c => c.Date, endDate.Value);
            }

            var checks = this.checks.Find(filter).ToList();

            var groupedData = checks.SelectMany(c => c.Items.Select(i => new
            {
                c.Date,
                Category = i.Category,
                i.Quantity

            }))
            .GroupBy(x => new { x.Date.Date, x.Category })
            .Select(g => new SalesData
            {
                Category = g.Key.Category,
                TotalSales=Convert.ToInt32(g.Sum(i => i.Quantity))
            })
            .ToList();

            return groupedData;
        }
        public (List<SalesCategoryData> salesCategoryData, SalesSummaryData salesSummaryData) GetSalesSummaryByCategory(string userId, DateTime? startDate, DateTime? endDate)
        {
            var query = checks.Find(c => c.UserId == userId).ToEnumerable();

            if (startDate.HasValue)
            {
                query = query.Where(c => c.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(c => c.Date <= endDate.Value);
            }

            var salesCategoryData = query.SelectMany(c => c.Items)
                                         .Where(i => !string.IsNullOrEmpty(i.Category))
                                         .GroupBy(i => i.Category)
                                         .Select(g => new SalesCategoryData
                                         {
                                             Category = g.Key,
                                             TotalSales = Convert.ToSingle(Math.Round(g.Sum(i => i.TotalPrice))),
                                             TotalProfit = Math.Round(g.Sum(i => i.Quantity * (i.Price - i.PurchasePrice))),
                                             SalesCount = g.Sum(i => (int)i.Quantity)
                                         })
                                         .ToList();

            var totalSalesCount = query.Sum(c => c.Items.Sum(i => (int)i.Quantity));
            var totalSalesAmount = query.Sum(c => c.Sum);
            var totalProfit = query.Sum(c => c.Profit);
            var averageCheck = query.Any() ? query.Average(c => c.Sum) : 0;

            var salesSummaryData = new SalesSummaryData
            {
                TotalSalesCount = totalSalesCount,
                TotalSalesAmount = Math.Round(totalSalesAmount),
                TotalProfit = Math.Round(totalProfit),
                AverageCheck = Math.Round(averageCheck)
            };

            return (salesCategoryData, salesSummaryData);
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
                        orderItem.Price = item.DiscountedPrice;
                        orderItem.Name = item.Name;
                        orderItem.Category = item.Category;
                        orderItem.PurchasePrice = item.PurchPrice;

                        double itemTotal = Math.Round(orderItem.Quantity * orderItem.Price, 2);
                        totalSum += itemTotal;

                        double itemTotalProfit = Math.Round(orderItem.Quantity * item.MarkupPriceNumeric, 2);
                        totalProfit += itemTotalProfit;

                        item.Available -= Math.Round(orderItem.Quantity, 2);
                        item.Available = Math.Round(item.Available, 2);
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
                Items = itemsList,
                Sum = Math.Round(totalSum, 2),
                Date = DateTime.Now,
                Profit = Math.Round(totalProfit, 2),
                UserId = model.UserId
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

            var itemsList = new List<OrderItemViewModel>();
            foreach (var orderItem in check.Items)
            {
                itemsList.Add(new OrderItemViewModel
                {
                    Id = orderItem.Id,
                    Name = orderItem.Name,
                    Quantity = orderItem.Quantity,
                    Price = orderItem.Price
                });
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
                    item.Available = Math.Round(item.Available, 2);
                    var filter = Builders<Item>.Filter.Eq("Id", item.Id);
                    var update = Builders<Item>.Update.Set("Available", item.Available);
                    items.UpdateOne(filter, update);
                }
            }
        }
    }
}
