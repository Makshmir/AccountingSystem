using AccountingSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;

namespace AccountingSystem.Controllers
{
    public class TrendController : Controller
    {
        private readonly CheckService _checkService;

        public TrendController(CheckService checkService)
        {
            _checkService = checkService;
        }

        public IActionResult Index(DateTime? startDate, DateTime? endDate)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var (salesCategoryData, salesSummaryData) = _checkService.GetSalesSummaryByCategory(userId, startDate, endDate);
            ViewBag.SalesSummaryData = salesSummaryData;
            return View(salesCategoryData);
        }
    }
}
