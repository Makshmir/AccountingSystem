﻿using AccountingSystem.Models;
using AccountingSystem.Services;
using IronBarCode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using System.Security.Claims;

namespace AccountingSystem.Controllers
{
    [Authorize]
    public class itemsController : Controller
    {
        private readonly ItemService itemService;
        private readonly SupplierService supplierService;

        public itemsController(ItemService itemService, SupplierService supplierService)
        {
            this.itemService = itemService;
            this.supplierService = supplierService;
        }

        // GET: itemsController
        public ActionResult Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return View(itemService.GetByUserId(userId));
        }

        // GET: itemsController/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var item = itemService.Get(id, userId);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // GET: itemsController/Create
        [HttpGet]
        public ActionResult Create()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            dynamic model = new ExpandoObject();
            model.Items = new Item();
            model.Supplier = supplierService.GetByUserId(userId); // Assuming this method returns a list of suppliers

            return View(model);
        }

        // POST: items/Create/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(IFormCollection form)
        {
            dynamic model = new ExpandoObject();
            model.Items = new Item();
            model.Items.Name = form["Items.Name"];
            model.Items.Category = form["Items.Category"];
            model.Items.Unit = form["Items.Unit"];
            model.Items.Available = Convert.ToDouble(form["Items.Available"]);
            model.Items.Barcode = form["Items.Barcode"];
            model.Items.PurchPrice = Convert.ToDouble(form["Items.PurchPrice"]);
            model.Items.Price = Convert.ToDouble(form["Items.Price"]);
            model.Items.ImageUrl = form["Items.ImageUrl"];
            model.Items.SupplierId = form["Items.SupplierId"];
            model.Items.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            model.Items.MarkupPriceNumeric = Math.Round((model.Items.Price - model.Items.PurchPrice), 2);
            model.Items.MarkupPriceInterest = Math.Round((model.Items.Price / model.Items.PurchPrice - 1) * 100);

            itemService.Create(model.Items);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var item = itemService.Get(id, userId);
            if (item == null)
            {
                return NotFound();
            }

            var viewModel = new ItemViewModel
            {
                Item = item,
                Suppliers = supplierService.GetByUserId(userId)
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Item item)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            item.MarkupPriceNumeric = item.Price - item.PurchPrice;
            item.MarkupPriceInterest=Math.Round((item.Price / item.PurchPrice *100),2);
            if (ModelState.IsValid)
            {
                item.UserId = userId;
                itemService.Update(item.Id, item, userId);
                return RedirectToAction(nameof(Index));
            }

            // Вивести помилки валідації
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }

            // Create an instance of ItemViewModel and populate it
            var viewModel = new ItemViewModel
            {
                Item = item,
                Suppliers = supplierService.GetByUserId(userId)
            };

            return View(viewModel);
        }

        // GET: items/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var item = itemService.Get(id, userId);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // POST: items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var item = itemService.Get(id, userId);

                if (item == null)
                {
                    return NotFound();
                }

                itemService.Remove(item.Id, userId);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
