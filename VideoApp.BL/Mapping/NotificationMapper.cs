using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoApp.BL.BLModels;
using VideoApp.BL.DALModels;

namespace VideoApp.BL.Mapping
{
    public static class NotificationMapper
    {
        public static IEnumerable<Notification> MapToDal(IEnumerable<BLNotification> blNotification) =>
          blNotification.Select(x => MapToDal(x));


        public static Notification MapToDal(BLNotification blNotification)
        {
            return new Notification
            {
                Id = blNotification.Id,
                CreatedAt = blNotification.CreatedAt,
                ReceiverEmail = blNotification.ReceiverEmail,
                Subject = blNotification.Subject,
                Body = blNotification.Body,
                SentAt = blNotification.SentAt
            };
        }
        //public static IEnumerable<BLNotification> MapToBl(IEnumerable<Notification> dalNotification) =>
        //dalNotification.Select(x => MapToBl(x));

        //public static BLNotification MapToBl(Notification dalNotification)
        //{
        //    return new BLNotification
        //    {
        //        Id = dalNotification.Id,
        //        CreatedAt = dalNotification.CreatedAt,
        //        ReceiverEmail = dalNotification.ReceiverEmail,
        //        Subject = dalNotification.Subject,
        //        Body = dalNotification.Body,
        //        SentAt = dalNotification.SentAt

        //    };
        //}
    }
}
