using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace TakeAnElfie.Web.Hubs
{
    [HubName("ElfieHub")]
    public class ElfieHub : Hub
    {
        public void Send(string message)
        {
            Clients.Caller.showMessage("thanks for: " + message);
        }
    }
}