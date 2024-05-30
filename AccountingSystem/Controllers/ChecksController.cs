using AccountingSystem.Models;
using AccountingSystem.Services;
using IronBarCode;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;

namespace AccountingSystem.Controllers
{
    public class checksController : Controller
    {
        private readonly CheckService checkService;
        private readonly ItemService itemService;

        public checksController(CheckService checkService, ItemService itemService)
        {
            this.checkService = checkService;
            this.itemService = itemService;
        }

        public IActionResult GetDetails(string id)
        {
            var check = checkService.Get(id);
            if (check == null)
            {
                return NotFound();
            }
            return PartialView("_CheckDetails", check);
        }


        // GET: itemsController
        public ActionResult Index()
        {
            return View(checkService.Get());
        }




        // GET: itemsController/Create
        public ActionResult Create()
        {
            dynamic mymodel = new ExpandoObject();
            mymodel.Items = itemService.Get();

            mymodel.Checks = checkService.Get();
            return View(mymodel);
        }

        // POST: items/Create/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(OrderViewModel model)
        {
            foreach (var item in model.Items)
            {
                Console.WriteLine($"Item ID: {item.Id}, Quantity: {item.Quantity}");
            }

            checkService.Create(model);

            return RedirectToAction(nameof(Index));
        }





       public IActionResult Details(string id)
        {
            var checkDetails = checkService.GetCheckDetails(id);
            if (checkDetails == null)
            {
                return NotFound();
            }
            return View(checkDetails);
        }


        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = checkService.Get(id);
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
                var item = checkService.Get(id);

                if (item == null)
                {
                    return NotFound();
                }
                var deletedCheck = checkService.Get(id);

                checkService.Delete(item.Id);
                checkService.ReturnItemsToStock(deletedCheck.Items);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }




    }
}
