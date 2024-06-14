using AccountingSystem.Models;
using AccountingSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using System.Security.Claims;

namespace AccountingSystem.Controllers
{
    [Authorize]
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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var check = checkService.Get(id, userId);
            if (check == null)
            {
                return NotFound();
            }
            return PartialView("_CheckDetails", check);
        }

        // GET: checksController
        public ActionResult Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return View(checkService.GetByUserId(userId));
        }

        [HttpGet]
        public ActionResult Create()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            dynamic mymodel = new ExpandoObject();
            mymodel.Items = itemService.GetByUserId(userId);
            mymodel.Checks = checkService.GetByUserId(userId);
            return View(mymodel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(OrderViewModel model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            model.UserId = userId;
            checkService.Create(model);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var checkDetails = checkService.GetCheckDetails(id, userId);
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

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var item = checkService.Get(id, userId);
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
                var item = checkService.Get(id, userId);

                if (item == null)
                {
                    return NotFound();
                }

                var deletedCheck = checkService.Get(id, userId);

                checkService.Delete(item.Id, userId);
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
