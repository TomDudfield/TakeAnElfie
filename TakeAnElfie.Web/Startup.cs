using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(TakeAnElfie.Web.Startup))]
namespace TakeAnElfie.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalHost.Configuration.MaxIncomingWebSocketMessageSize = 10485760;
            app.MapSignalR();
        }
    }
}