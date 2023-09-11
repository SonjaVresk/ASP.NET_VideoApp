using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoApp.BL.BLModels;

namespace VideoApp.BL.Repositories
{
    public interface INotificationRepository
    {
        int GetUnsentNotificationCount();

        List<BLNotification> GetUnsentNotifications();

        //void Send(BLNotification notification);

        IEnumerable<BLNotification> GetNotifications();

        BLNotification GetNotification(int id);

        BLNotification CreateNotification(BLNotification blNotification);
    }
}
