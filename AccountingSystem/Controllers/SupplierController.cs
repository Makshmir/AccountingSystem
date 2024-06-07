using AccountingSystem.Models;
using AccountingSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountingSystem.Controllers
{

    [Authorize]
    public class SupplierController : Controller
    {
         private readonly SupplierService supplierService;


        public SupplierController(SupplierService supplierService)
        {
            this.supplierService = supplierService;
        }

        public ActionResult Index()
        {
            return View(supplierService.Get());









        }


        public ActionResult Create()
        {
            return View();
        }

        // POST: items/Create/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Supplier supplier)
        {

            supplierService.Create(supplier);
            return RedirectToAction(nameof(Index));
        }

        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = supplierService.Get(id);
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
                var item = supplierService.Get(id);

                if (item == null)
                {
                    return NotFound();
                }

                supplierService.Remove(item.Id);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }





    }



}
