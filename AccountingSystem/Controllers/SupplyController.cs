using AccountingSystem.Models;
using AccountingSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

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
            var supplies = _supplyService.Get();
            var suppliers = _supplierService.Get().ToDictionary(s => s.Id, s => s.Name);
            var items = _itemService.Get().ToDictionary(i => i.Id, i => i.Name);

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
            ViewBag.Suppliers = _supplierService.Get();
            ViewBag.Items = _itemService.Get();
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
                SupplyDate = DateTime.Parse(form["SupplyDate"])
            };
            model.Supply.Items = new List<SupplyItem>();

            // Обробка декількох товарів
            int itemCount = Convert.ToInt32(form["ItemCount"]);
            for (int i = 0; i < itemCount; i++)
            {
                var item = new SupplyItem
                {
                    //Id = ObjectId.GenerateNewId().ToString(),
                    ItemId = form[$"Items[{i}].ItemId"],
                    Quantity = Convert.ToInt32(form[$"Items[{i}].Quantity"]),
                    PurchasePrice= Convert.ToDouble(form[$"Items[{i}].PurchasePrice"])

                };
                model.Supply.Items.Add(item);

                // Оновлення кількості товарів у базі даних
                _itemService.UpdateQuantity(item.ItemId, item.Quantity);
            }

            // Збереження поставки
            _supplyService.Create(model.Supply);

            return RedirectToAction(nameof(Index));
        }





        public IActionResult Edit(string id)
        {
            var supply = _supplyService.Get(id);
            if (supply == null)
            {
                return NotFound();
            }
            ViewBag.Suppliers = _supplierService.Get();
            ViewBag.Items = _itemService.Get();
            return View(supply);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, Supply supply)
        {
            if (id != supply.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _supplyService.Update(id, supply);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Suppliers = _supplierService.Get();
            ViewBag.Items = _itemService.Get();
            return View(supply);
        }

        public IActionResult Delete(string id)
        {
            var supply = _supplyService.Get(id);
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
            var supply = _supplyService.Get(id);
            if (supply == null)
            {
                return NotFound();
            }
            _supplyService.Remove(id);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(string id)
        {
            var supply = _supplyService.Get(id);
            if (supply == null)
            {
                return NotFound();
            }
            return View(supply);
        }
    }
}
