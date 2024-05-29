using AccountingSystem.Models;
using AccountingSystem.Services;
using IronBarCode;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccountingSystem.Controllers
{
    public class itemsController : Controller
    {
        private readonly ItemService itemService;

        public itemsController(ItemService itemService)
        {
            this.itemService = itemService;
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
        }

        // GET: itemsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: items/Create/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Item item)
        {
            item.MarkupPriceNumeric = Math.Round((item.Price - item.PurchPrice), 2);
            item.MarkupPriceInterest=Math.Round((item.Price / item.PurchPrice-1)*100);
            itemService.Create(item);
            return RedirectToAction(nameof(Index));
        }

        // GET: itemsController/Edit/5
        public ActionResult Edit(string id)
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

        // POST: items/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, Item item)
        {
            if (id != item.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                itemService.Update(id, item);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(item);
            }
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
