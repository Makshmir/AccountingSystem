using AccountingSystem.Models;
using AccountingSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AccountingSystem.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly NotificationService _notificationService;

        public NotificationController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public JsonResult GetNotifications()
        {
            var notifications = _notificationService.GetAll();

            var today = DateTime.Today;
            notifications = notifications.Where(n => n.EventDate >= today).ToList();
            notifications = notifications.Where(n => (n.EventDate - today).TotalDays <= 20).ToList();

            return Json(notifications);
        }

        [HttpGet]
        public JsonResult GetNotificationCount()
        {
            var notifications = _notificationService.GetAll();
            var today = DateTime.Today;
            notifications = notifications.Where(n => n.EventDate >= today).ToList();
            notifications = notifications.Where(n => (n.EventDate - today).TotalDays <= 20).ToList();
            var count = notifications.Count;
            return Json(count);
        }

        public IActionResult Index()
        {
            return View(_notificationService.GetAll());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Notification notification)
        {
            notification.EventDate = notification.EventDate.AddDays(1);
            if (ModelState.IsValid)
            {
                _notificationService.Create(notification);
                return RedirectToAction(nameof(Index));
            }
            return View(notification);
        }

        public IActionResult Edit(string id)
        {
            var notification = _notificationService.Get(id);
            if (notification == null)
            {
                return NotFound();
            }
            return View(notification);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, Notification notification)
        {
            notification.EventDate = notification.EventDate.AddDays(1);
            if (id != notification.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _notificationService.Update(notification);
                return RedirectToAction(nameof(Index));
            }
            return View(notification);
        }

        public IActionResult Details(string id)
        {
            var notification = _notificationService.Get(id);
            if (notification == null)
            {
                return NotFound();
            }
            return View(notification);
        }

        public IActionResult Delete(string id)
        {
            var notification = _notificationService.Get(id);
            if (notification == null)
            {
                return NotFound();
            }
            return View(notification);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            var notification = _notificationService.Get(id);
            if (notification == null)
            {
                return NotFound();
            }

            _notificationService.Remove(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
