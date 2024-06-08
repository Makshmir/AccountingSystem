using AccountingSystem.Models;
using AccountingSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return View(supplierService.GetByUserId(userId));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Supplier supplier)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            supplier.UserId = userId;
            supplierService.Create(supplier);
            return RedirectToAction(nameof(Index));
        }

        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var item = supplierService.Get(id, userId);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var item = supplierService.Get(id, userId);

                if (item == null)
                {
                    return NotFound();
                }

                supplierService.Remove(item.Id, userId);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
