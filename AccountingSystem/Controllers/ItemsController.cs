using AccountingSystem.Models;
using AccountingSystem.Services;
using IronBarCode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using System.Security.Claims;
using System.Text;

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

        public IActionResult GeneratePdf(string[] itemIds)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (itemIds == null || itemIds.Length == 0)
            {
                return BadRequest("No items selected.");
            }

            var items = itemService.GetItemsByIds(itemIds, userId);

            if (items == null || !items.Any())
            {
                return NotFound();
            }

            var priceTagsHtml = new StringBuilder();

            foreach (var item in items)
            {
                IronBarCode.GeneratedBarcode myBarcode = IronBarCode.BarcodeWriter.CreateBarcode(item.Barcode, IronBarCode.BarcodeEncoding.Code128);
                myBarcode.ResizeTo(200, 40);
                myBarcode.SetMargins(2, 2, 2, 2);
                string DataURL = myBarcode.ToDataUrl();

                string itemHtml = $@"
                <div class='price-tag'>
                    <div class='price-element'>
                        <span class='price-label'>Назва товару:</span>
                        <span class='price-name'>{item.Name}</span>
                    </div>
                    <div class='price-element'>
                        {(item.Discount != 0 ? $"<s class='original-price'>{item.Price} грн</s>" : "")}
                        <span class='discounted-price'>{item.DiscountedPrice} грн / {item.Unit}</span>
                    </div>
                    <img src='{DataURL}' class='barcode-image' />
                </div>";
                priceTagsHtml.Append(itemHtml);
            }

            string htmlContent = $@"
                <html>
                <head>
                    <style>
                        .price-title {{ font-size: 1.5em; font-weight: bold; text-align: center; }}
                        .price-details {{ max-width: 1000px; margin: 0 auto; display: flex; flex-wrap: wrap; gap: 10px; }}
                        .price-element {{ margin-bottom: 5px; }}
                        .price-label {{ font-weight: bold; color: #555; font-size: 0.8em; }}
                        .price-name {{ font-size: 1.2em; color: #333; }}
                        .original-price {{ font-size: 0.8em; color: #999; text-decoration: line-through; margin-right: 5px; }}
                        .discounted-price {{ font-size: 1.2em; color: #e74c3c; }}
                        .barcode-image {{ margin-top: 10px; display: block; max-width: 100%; height: auto; }}
                        .price-tag {{ width: 48%; page-break-inside: avoid; border: 1px solid #ddd; padding: 10px; box-sizing: border-box; margin-bottom: 10px; }}
                    </style>
                </head>
                <body>
                    <h1 class='price-title'>Цінники</h1>
                    <div class='price-details'>
                        {priceTagsHtml.ToString()}
                    </div>
                </body>
                </html>";
            var Renderer = new IronPdf.HtmlToPdf();
            var pdf = Renderer.RenderHtmlAsPdf(htmlContent);
            var pdfBytes = pdf.BinaryData;
            var fileName = $"PriceTags_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }

        // GET: itemsController/Create
        [HttpGet]
        public ActionResult Create()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            dynamic model = new ExpandoObject();
            model.Items = new Item();
            model.Supplier = supplierService.GetByUserId(userId); 
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
            model.Items.Discount= Convert.ToDouble(form["Items.Discount"]);
            model.Items.MarkupPriceNumeric = Convert.ToDouble(form["Items.MarkupPriceNumeric"]);
            model.Items.MarkupPriceInterest = Convert.ToDouble(form["Items.MarkupPriceInterest"]);
            model.Items.DiscountedPrice = Convert.ToDouble(form["Items.DiscountedPrice"]);
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

            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }

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

        [HttpPost]
        public IActionResult GenerateRandomDiscounts()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var items = itemService.GetByUserId(userId);
            var random = new Random();

            foreach (var item in items)
            {
                if (item.Price <= item.PurchPrice)
                {
                    item.Discount = 0;
                    item.DiscountedPrice = item.Price;
                }
                else
                {
                    var maxDiscount = (int)Math.Floor(100 * (1 - (item.PurchPrice * 1.1) / item.Price));

                    if (maxDiscount < 5)
                    {
                        item.Discount = 0;
                        item.DiscountedPrice = item.Price;
                    }
                    else
                    {
                        var discount = random.Next(5, Math.Min(31, maxDiscount + 1));
                        var discountedPrice = Math.Round(item.Price - (item.Price * (discount / 100.0)), 2);

                        item.Discount = discount;
                        item.DiscountedPrice = discountedPrice;
                    }
                }

                item.MarkupPriceNumeric = Math.Round(item.DiscountedPrice - item.PurchPrice, 2);
                item.MarkupPriceInterest = Math.Round((item.PurchPrice != 0) ? ((item.MarkupPriceNumeric / item.PurchPrice) * 100) : 0, 2);

                itemService.Update(item.Id, item, userId);
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult RemoveAllDiscounts()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var items = itemService.GetByUserId(userId);

            foreach (var item in items)
            {
                item.Discount = 0;
                item.DiscountedPrice = item.Price;
                itemService.Update(item.Id, item, userId);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
