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
        private const string CameraGroup = "camera";

        public void ConnectCamera()
        {
            Groups.Add(Context.ConnectionId, CameraGroup);
        }
        
        public void TakeImage()
        {
            Clients.Group(CameraGroup).takeImage(Context.ConnectionId);
        }

        public void ProcessImage(string userId, string image)
        {
            Clients.Client(userId).reviewImage(image);
        }

        public void ApproveImage()
        {

            Clients.Caller.showTweet("tweet");
        }
    }
}