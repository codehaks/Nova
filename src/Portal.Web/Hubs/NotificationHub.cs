using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Web.Hubs
{
    public class NotificationHub:Hub<INotificationHub>
    {
    }

    public interface INotificationHub
    {
        Task SendNotification(string message);
    }
}
