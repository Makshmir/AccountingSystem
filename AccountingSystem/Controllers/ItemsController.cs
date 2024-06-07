using AccountingSystem.Models;
using AccountingSystem.Services;
using IronBarCode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;

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
            return View(itemService.Get());
        }








        // GET: itemsController/Details/5
        public ActionResult Details(string id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var item = itemService.Get(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);



            //var item = itemService.Get(id);
            //if (item == null)
            //{
            //    return NotFound();
            //}

            //var supplier = supplierService.Get(item.SupplierId);
            //var model = new ItemDetailsViewModel
            //{
            //    Item = item,
            //    SupplierName = supplier?.Name
            //};

            //return View(model);



        }

        // GET: itemsController/Create
        [HttpGet]
        public ActionResult Create()
        {

            dynamic model = new ExpandoObject();
            model.Items = new Item();


            model.Supplier = supplierService.Get(); // Assuming this method returns a list of suppliers

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


            model.Items.MarkupPriceNumeric = Math.Round((model.Items.Price - model.Items.PurchPrice), 2);
            model.Items.MarkupPriceInterest=Math.Round((model.Items.Price / model.Items.PurchPrice-1)*100);
            itemService.Create(model.Items);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var item = itemService.Get(id);
            if (item == null)
            {
                return NotFound();
            }

            var viewModel = new ItemViewModel
            {
                Item = item,
                Suppliers = supplierService.Get()
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Item item)
        {
            if (ModelState.IsValid)
            {
                itemService.Update(item.Id, item);
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
                Suppliers = supplierService.Get()
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

            var item = itemService.Get(id);
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
                var item = itemService.Get(id);

                if (item == null)
                {
                    return NotFound();
                }

                itemService.Remove(item.Id);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
