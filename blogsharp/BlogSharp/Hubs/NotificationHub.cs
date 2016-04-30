using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace BlogSharp
{
    public class NotificationHub : Hub
    {
        private IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();

        public void loginProgress()
        {
            // check if the name corresponds with a logged in user
            hubContext.Clients.All.notifyUsers();


            // otherwise do nothing
            
            
        }
    }
}