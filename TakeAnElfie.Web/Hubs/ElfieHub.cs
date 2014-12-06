using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

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

            Bitmap bitmap = new Bitmap(Path.Combine(HttpContext.Current.Server.MapPath("~/"), "/Content/Images/logo.png"));
            MemoryStream memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);

            StorageCredentials credentials = new StorageCredentials("takeanelfie", "u3ihGBt89nJdjuVSbGI3I8Ggu5ff80RkuItFvCL1GRI5f46Yx4fQNYvdxofqUdBqamYbPUtT9Yx7nq5QXVJqOA==");
            CloudBlobContainer container = new CloudBlobContainer(new Uri("https://takeanelfie.blob.core.windows.net/originals"), credentials);
            CloudBlockBlob blob = container.GetBlockBlobReference(userId);
            blob.UploadFromStream(memoryStream);
        }

        public void ApproveImage()
        {

            Clients.Caller.showTweet("tweet");
        }
    }
}