using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Web.Hubs
{
    public class MonitoringHub:Hub<IMonitoringHub>
    {
    }

    public interface IMonitoringHub
    {
        Task SendHealthUpdate(string message);
    }
}
