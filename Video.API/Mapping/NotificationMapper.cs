using Video.API.APIModels;
using VideoApp.BL.BLModels;
using VideoApp.BL.DALModels;

namespace Video.API.Mapping
{
    public static class NotificationMapper
    {
        public static IEnumerable<APINotification> MapToAPI(IEnumerable<BLNotification> blNotification) =>
         blNotification.Select(x => MapToAPI(x));

        public static APINotification MapToAPI(BLNotification blNotification)
        {
            return new APINotification
            {
                ReceiverEmail = blNotification.ReceiverEmail,
                Subject = blNotification.Subject,
                Body = blNotification.Body
            };
        }

        public static IEnumerable<BLNotification> MapToBl(IEnumerable<APINotification> apiNotifications) =>
          apiNotifications.Select(x => MapToBl(x));

        public static BLNotification MapToBl(APINotification apiNotifications)
        {
            return new BLNotification
            {
                ReceiverEmail = apiNotifications.ReceiverEmail,
                Subject = apiNotifications.Subject,
                Body = apiNotifications.Body
            };
        }
    }
}
