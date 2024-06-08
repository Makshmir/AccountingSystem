using AccountingSystem.Models;
using AccountingSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Claims;
using Newtonsoft.Json;

namespace AccountingSystem.Controllers
{
    [Authorize]
    public class SupplyController : Controller
    {
        private readonly SupplyService _supplyService;
        private readonly SupplierService _supplierService;
        private readonly ItemService _itemService;

        public SupplyController(SupplyService supplyService, SupplierService supplierService, ItemService itemService)
        {
            _supplyService = supplyService;
            _supplierService = supplierService;
            _itemService = itemService;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var supplies = _supplyService.GetByUserId(userId);
            var suppliers = _supplierService.GetByUserId(userId).ToDictionary(s => s.Id, s => s.Name);
            var items = _itemService.GetByUserId(userId).ToDictionary(i => i.Id, i => i.Name);

            var model = supplies.Select(supply => new SupplyViewModel
            {
                Id = supply.Id,
                SupplierName = suppliers.ContainsKey(supply.SupplierId) ? suppliers[supply.SupplierId] : "Unknown",
                SupplyDate = supply.SupplyDate,
                TotalAmount = supply.TotalAmount,
                Items = supply.Items.Select(item => new SupplyItemViewModel
                {
                    ItemName = items.ContainsKey(item.ItemId) ? items[item.ItemId] : "Unknown",
                    Quantity = item.Quantity,
                    PurchasePrice = item.PurchasePrice,
                    TotalPrice = item.TotalPrice
                }).ToList()
            }).ToList();

            return View(model);
        }

        public IActionResult Create()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            ViewBag.Suppliers = _supplierService.GetByUserId(userId);
            ViewBag.Items = _itemService.GetByUserId(userId);
            return View(new Supply());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(IFormCollection form)
        {
            dynamic model = new ExpandoObject();
            model.Supply = new Supply
            {
                Id = ObjectId.GenerateNewId().ToString(),
                SupplierId = form["SupplierId"],
                SupplyDate = DateTime.Parse(form["SupplyDate"]),
                UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            };
            model.Supply.Items = new List<SupplyItem>();
            int itemCount = Convert.ToInt32(form["ItemCount"]);
            for (int i = 0; i < itemCount; i++)
            {
                var item = new SupplyItem
                {
                    ItemId = form[$"Items[{i}].ItemId"],
                    Quantity = Convert.ToInt32(form[$"Items[{i}].Quantity"]),
                    PurchasePrice = Convert.ToDouble(form[$"Items[{i}].PurchasePrice"])
                };
                model.Supply.Items.Add(item);
                model.Supply.TotalAmount += item.TotalPrice;
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _itemService.UpdateQuantityAndPurchasePrice(item.ItemId, item.Quantity, item.PurchasePrice, userId);
            }

            _supplyService.Create(model.Supply);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var supply = _supplyService.GetByUserId(id, userId);
            if (supply == null)
            {
                return NotFound();
            }
            ViewBag.Suppliers = _supplierService.GetByUserId(userId);
            ViewBag.Items = _itemService.GetByUserId(userId);
            ViewBag.ItemsJson = JsonConvert.SerializeObject(ViewBag.Items);
            return View(supply);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, Supply supply)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (id != supply.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                supply.UserId = userId; // Ensure that the UserId is set correctly
                _supplyService.Update(id, supply);

                foreach (var item in supply.Items)
                {
                    _itemService.UpdateQuantityAndPurchasePrice(item.ItemId, item.Quantity, item.PurchasePrice, userId);
                }

                return RedirectToAction(nameof(Index));
            }
            ViewBag.Suppliers = _supplierService.GetByUserId(userId);
            ViewBag.Items = _itemService.GetByUserId(userId);
            return View(supply);
        }

        public IActionResult Delete(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var supply = _supplyService.GetByUserId(id, userId);
            if (supply == null)
            {
                return NotFound();
            }
            return View(supply);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var supply = _supplyService.GetByUserId(id, userId);
            if (supply == null)
            {
                return NotFound();
            }
            _supplyService.Remove(id, userId);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var supply = _supplyService.GetByUserId(id, userId);
            if (supply == null)
            {
                return NotFound();
            }
            return View(supply);
        }
    }
}
