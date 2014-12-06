using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
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
            const string containerUrl = "https://takeanelfie.blob.core.windows.net/originals/";
            var base64Data = Regex.Match(image, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
            var imageBytes = Convert.FromBase64String(base64Data);
            var imageName = userId + ".png";

            using (MemoryStream memoryStream = new MemoryStream(imageBytes))
            {
                memoryStream.Seek(0, SeekOrigin.Begin);

                StorageCredentials credentials = new StorageCredentials("takeanelfie", "u3ihGBt89nJdjuVSbGI3I8Ggu5ff80RkuItFvCL1GRI5f46Yx4fQNYvdxofqUdBqamYbPUtT9Yx7nq5QXVJqOA==");
                CloudBlobContainer container = new CloudBlobContainer(new Uri(containerUrl), credentials);
                CloudBlockBlob blob = container.GetBlockBlobReference(imageName);
                blob.UploadFromStream(memoryStream);
            }

            Clients.Client(userId).reviewImage(containerUrl + imageName);
        }

        public void ApproveImage()
        {
            Clients.Caller.showTweet("tweet");
        }
    }
}