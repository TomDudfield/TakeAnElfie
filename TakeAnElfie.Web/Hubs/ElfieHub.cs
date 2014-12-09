using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Tweetinvi;

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
            const string containerName = "originals";
            var base64Data = Regex.Match(image, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
            var imageBytes = Convert.FromBase64String(base64Data);
            var imageName = userId + ".png";

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["azureBlobStorage"].ConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            container.CreateIfNotExists();
            CloudBlockBlob blob = container.GetBlockBlobReference(imageName);

            using (MemoryStream memoryStream = new MemoryStream(imageBytes))
            {
                memoryStream.Seek(0, SeekOrigin.Begin);
                blob.UploadFromStream(memoryStream);
                
#if DEBUG
                Clients.Client(userId).reviewImage(blob.Uri);
#else
                Clients.Client(userId).reviewImage(ConfigurationManager.AppSettings["CdnUrl"] + containerName + "/" + imageName);
#endif
            }
        }

        public void ApproveImage(string image)
        {
            const string containerName = "processed";

            WebRequest request = WebRequest.Create(image);
            WebResponse response = request.GetResponse();
            Bitmap originalBitmap = null;

            using (var responseStream = response.GetResponseStream())
            {
                if (responseStream != null)
                    originalBitmap = new Bitmap(responseStream);
            }

            if (originalBitmap == null)
                return;

            Bitmap overlayBitmap = new Bitmap(HttpContext.Current.Server.MapPath("~/Content/Images/assets/hat 1.png"));
            Graphics combinedGraphic = Graphics.FromImage(originalBitmap);
            combinedGraphic.DrawImage(overlayBitmap, 0, 0, overlayBitmap.Width, overlayBitmap.Height);

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["azureBlobStorage"].ConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            container.CreateIfNotExists();

            var imageName = Context.ConnectionId + ".png";
            CloudBlockBlob blob = container.GetBlockBlobReference(imageName);
            string imageUrl;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                originalBitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                memoryStream.Seek(0, SeekOrigin.Begin);
                blob.UploadFromStream(memoryStream);

#if DEBUG
                imageUrl = blob.Uri.ToString();
#else
                imageUrl = ConfigurationManager.AppSettings["CdnUrl"] + containerName + "/" + imageName;
#endif
            }

            var userAccessToken = ConfigurationManager.AppSettings["UserAccessToken"];
            var userAccessSecret = ConfigurationManager.AppSettings["UserAccessSecret"];
            var consumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
            var consumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];

            if (!string.IsNullOrEmpty(userAccessToken) && !string.IsNullOrEmpty(userAccessSecret) &&
                !string.IsNullOrEmpty(consumerKey) && !string.IsNullOrEmpty(consumerSecret))
            {
                var twitterCredentials = TwitterCredentials.CreateCredentials(userAccessToken, userAccessSecret, consumerKey, consumerSecret);
                TwitterCredentials.ExecuteOperationWithCredentials(twitterCredentials, () =>
                {
                    var tweet = Tweet.CreateTweet(imageUrl);
                    tweet.Publish();
                });
            }

            Clients.Caller.showTweet(imageUrl);
        }
    }
}