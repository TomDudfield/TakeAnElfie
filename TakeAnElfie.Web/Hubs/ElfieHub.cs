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
        private const string ContainerUrl = "https://takeanelfie.blob.core.windows.net/";
        private const string ContainerName = "takeanelfie";
        private const string CdnUrl = "http://az697179.vo.msecnd.net/";

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
            //StorageCredentials credentials = new StorageCredentials("takeanelfie", "u3ihGBt89nJdjuVSbGI3I8Ggu5ff80RkuItFvCL1GRI5f46Yx4fQNYvdxofqUdBqamYbPUtT9Yx7nq5QXVJqOA==");
            // CloudBlobContainer container = new CloudBlobContainer(new Uri(ContainerUrl + containerFolder), credentials);
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
                Clients.Client(userId).reviewImage(CdnUrl + containerName + "/" + imageName);
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
                {
                    originalBitmap = new Bitmap(responseStream);
                }
            }

            if (originalBitmap == null)
                return;;

            Bitmap overlayBitmap = new Bitmap(HttpContext.Current.Server.MapPath("~/Content/Images/assets/hat 1.png"));
            Graphics combinedGraphic = Graphics.FromImage(originalBitmap);
            combinedGraphic.DrawImage(overlayBitmap, 0, 0, overlayBitmap.Width, overlayBitmap.Height);

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["azureBlobStorage"].ConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            //StorageCredentials credentials = new StorageCredentials("takeanelfie", "u3ihGBt89nJdjuVSbGI3I8Ggu5ff80RkuItFvCL1GRI5f46Yx4fQNYvdxofqUdBqamYbPUtT9Yx7nq5QXVJqOA==");
            //CloudBlobContainer container = new CloudBlobContainer(new Uri(ContainerUrl + containerFolder), credentials);
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
                imageUrl = CdnUrl + containerName + "/" + imageName;
#endif
            }

            var twitterCredentials = TwitterCredentials.CreateCredentials("2834910083-hQCWvhAmnArzAxc80paU9ftNWtfeMaeGyHWvPzP", "3ZxjrZUCju54cBQcbp6kDE1gS6uFzAPf37kNFDAzF9WUl", "iGyrQI7U1Y8SjuXmuwFy78fZa", "L3prxTxJ2kjWlFVmWmXNyzuS8XzsOHF80kPwGutDtc8NvvueFq");
            TwitterCredentials.ExecuteOperationWithCredentials(twitterCredentials, () =>
            {
                var tweet = Tweet.CreateTweet(imageUrl);
                tweet.Publish();
            });

            Clients.Caller.showTweet(imageUrl);
        }
    }
}