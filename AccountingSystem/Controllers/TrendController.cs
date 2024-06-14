using AccountingSystem.Models;
using AccountingSystem.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public class TrendController : Controller
{
    private readonly CheckService _checkService;
    private readonly TrendPredictionService _trendPredictionService;

    public TrendController(CheckService checkService, TrendPredictionService trendPredictionService)
    {
        _checkService = checkService;
        _trendPredictionService = trendPredictionService;
    }

    public IActionResult Index(DateTime? startDate, DateTime? endDate)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var (salesCategoryData, salesSummaryData) = _checkService.GetSalesSummaryByCategory(userId, startDate, endDate);

        var sampleData = _checkService.GetSalesByCategoryDaily(userId, startDate, endDate);

        var salesData = sampleData.Select(data => new SalesData
        {
            Category = data.Category,
            TotalSales = Convert.ToSingle(data.TotalSales)
        }).ToList();

        _trendPredictionService.TrainModel(salesData);

        var trendCategoryScores = salesCategoryData.Select(data => new
        {
            Category = data.Category,
            Score = _trendPredictionService.Predict(data.Category)
        }).OrderByDescending(x => x.Score).ToList();
        ViewBag.SalesSummaryData = salesSummaryData;

        ViewBag.TrendCategoryScores = trendCategoryScores;

        return View(salesCategoryData);
    }
}
