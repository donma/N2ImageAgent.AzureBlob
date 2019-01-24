using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;

namespace N2ImageAgent.AzureBlob
{
    public static class BlobUtility
    {
        private static Microsoft.WindowsAzure.Storage.CloudStorageAccount CloudStorage;
        private static Microsoft.WindowsAzure.Storage.Blob.CloudBlobClient CloudBlobClient;
        private static Microsoft.WindowsAzure.Storage.Blob.CloudBlobContainer CloudBlobContainer;

        static BlobUtility()
        {
            CloudStorage = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(Startup.AzureStorageConnectionString);
            CloudBlobClient = CloudStorage.CreateCloudBlobClient();
            CloudBlobContainer = CloudBlobClient.GetContainerReference(Startup.BlobName);
            var res = CloudBlobContainer.CreateIfNotExistsAsync().Result;

        }
        
        public static Models.ImageInfo ReadInfoFromBlob(string id)
        {
            Microsoft.WindowsAzure.Storage.Blob.CloudBlobDirectory cloudBlobDirectory =
                CloudBlobContainer.GetDirectoryReference("source/info");

            var result = cloudBlobDirectory.GetBlockBlobReference(id + ".json").DownloadTextAsync().Result;

            return JsonConvert.DeserializeObject<Models.ImageInfo>(result);
        }


        public static Image DownloadFileFromBlob(string fileName, string blobPath = "source/images")
        {

            Microsoft.WindowsAzure.Storage.Blob.CloudBlobDirectory cloudBlobDirectory =
                CloudBlobContainer.GetDirectoryReference(blobPath);

            System.IO.Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "downloadsource");

            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "downloadsource" + Path.DirectorySeparatorChar + fileName))
            {
                cloudBlobDirectory.GetBlockBlobReference(fileName).DownloadToFileAsync(AppDomain.CurrentDomain.BaseDirectory + "downloadsource" + Path.DirectorySeparatorChar + fileName, System.IO.FileMode.Create).GetAwaiter().GetResult();
            }
            var res = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "downloadsource" + Path.DirectorySeparatorChar + fileName);

            return res;

        }


        public static void GetUriAndPermission(string fileName, out string fileUri, out string signUriPara, int keepSeconds, string blobPath = "source/images")
        {

            Microsoft.WindowsAzure.Storage.Blob.CloudBlobDirectory cloudBlobDirectory =
                CloudBlobContainer.GetDirectoryReference(blobPath);


            var res = CloudBlobContainer.GetPermissionsAsync().Result;

            var sharedPolicy = new Microsoft.WindowsAzure.Storage.Blob.SharedAccessBlobPolicy()
            {
                SharedAccessExpiryTime = DateTime.UtcNow.AddSeconds(keepSeconds),
                Permissions = Microsoft.WindowsAzure.Storage.Blob.SharedAccessBlobPermissions.Read,

            };
            signUriPara = CloudBlobContainer.GetSharedAccessSignature(sharedPolicy, null);

            fileUri = cloudBlobDirectory.GetBlockBlobReference(fileName).Uri.ToString();
        }

        public static bool IsFileExisted(string fileName, string blobPath = "source/images")
        {
            Microsoft.WindowsAzure.Storage.Blob.CloudBlobDirectory cloudBlobDirectory =
                CloudBlobContainer.GetDirectoryReference(blobPath);

            return cloudBlobDirectory.GetBlockBlobReference(fileName).ExistsAsync().Result;

        }

        public static void UpoloadImage(string id, string localImagePath, string filePath = "source/images/")
        {
            Microsoft.WindowsAzure.Storage.Blob.CloudBlobDirectory cloudBlobDirectory =
                 CloudBlobContainer.GetDirectoryReference(filePath);

            var bFileInfo = cloudBlobDirectory.GetBlockBlobReference(id + ".gif");

            bFileInfo.Properties.ContentType = "image/gif";

            bFileInfo.UploadFromFileAsync(localImagePath).GetAwaiter().GetResult();

        }


        public static void UpoloadImageSource(string filePath, string id)
        {
            Microsoft.WindowsAzure.Storage.Blob.CloudBlobDirectory cloudBlobDirectory =
                CloudBlobContainer.GetDirectoryReference("source/images/");

            var bFileInfo = cloudBlobDirectory.GetBlockBlobReference(id + ".gif");

            bFileInfo.Properties.ContentType = "image/gif";

            bFileInfo.UploadFromFileAsync(filePath).GetAwaiter().GetResult();

        }

        public static void UpoloadImageInfoSource(string filePath, string id)
        {
            Microsoft.WindowsAzure.Storage.Blob.CloudBlobDirectory cloudBlobDirectory =
               CloudBlobContainer.GetDirectoryReference("source/info/");

            var bFileInfo = cloudBlobDirectory.GetBlockBlobReference(id + ".json");
            bFileInfo.Properties.ContentType = "application/json; charset=utf-8";
            bFileInfo.UploadTextAsync(System.IO.File.ReadAllText(filePath)).GetAwaiter().GetResult();
            
        }

    }
}
