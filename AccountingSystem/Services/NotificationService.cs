using AccountingSystem.Models;
using MongoDB.Driver;

namespace AccountingSystem.Services
{
    public class NotificationService
    {
        private readonly IMongoCollection<Notification> _notifications;

        public NotificationService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("AccountingDb"));
            var database = client.GetDatabase("AccountingDb");
            _notifications = database.GetCollection<Notification>("Notifications");
        }

        public List<Notification> GetAll()
        {
            return _notifications.Find(notification => true).ToList();
        }

        public Notification Get(string id)
        {
            return _notifications.Find(notification => notification.Id == id).FirstOrDefault();
        }

        public Notification Create(Notification notification)
        {
            _notifications.InsertOne(notification);
            return notification;
        }


        public void Update(Notification notification)
        {
            _notifications.ReplaceOne(n => n.Id == notification.Id, notification);
        }
        public void Remove(string id)
        {
            _notifications.DeleteOne(notification => notification.Id == id);
        }
    }
}
