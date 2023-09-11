using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoApp.BL.BLModels;
using VideoApp.BL.DALModels;
using VideoApp.BL.Mapping;

namespace VideoApp.BL.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly RwaMoviesContext _dbContext;
        private readonly IMapper _mapper;

        public NotificationRepository(RwaMoviesContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public int GetUnsentNotificationCount()
        {
            int count = _dbContext.Notifications.Count(n => n.SentAt == null);
            return count;
        }

        public List<BLNotification> GetUnsentNotifications()
        {
            // Retrieve unsent notifications from the database
            var unsentNotifications = _dbContext.Notifications
                .Where(n => n.SentAt == null)
                .Select(n => new BLNotification
                {
                    Id = n.Id,
                    CreatedAt = n.CreatedAt,
                    ReceiverEmail = n.ReceiverEmail,
                    Subject = n.Subject,
                    Body = n.Body,
                    SentAt = n.SentAt
                })
                .ToList();

            return unsentNotifications;
        }


        //public void Send(BLNotification notification)
        //{
        //    Debug.WriteLine(notification);
        //}

        public IEnumerable<BLNotification> GetNotifications()
        {
            var dalNotifications = _dbContext.Notifications;

            var blNotifications = _mapper.Map<IEnumerable<BLNotification>>(dalNotifications);

            return blNotifications;
        }

        public BLNotification GetNotification(int id)
        {
            var dalNotification = _dbContext.Notifications.FirstOrDefault(x => x.Id == id);

            var blNotification = _mapper.Map<BLNotification>(dalNotification);            

            return blNotification;
        }

        public BLNotification CreateNotification(BLNotification blNotification)
        {

            var dalNotification = NotificationMapper.MapToDal(blNotification);

            dalNotification.CreatedAt = DateTime.UtcNow;

            _dbContext.Notifications.Add(dalNotification);

            _dbContext.SaveChanges();

            blNotification = _mapper.Map<BLNotification>(dalNotification);

            return (blNotification);

        }

    }
}
