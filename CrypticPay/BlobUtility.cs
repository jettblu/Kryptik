using Azure.Storage.Blobs;
using CrypticPay.Areas.Identity.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace CrypticPay
{
    public class BlobUtility
    {   
        CloudBlobClient BlobClient { get; set; }
        string ConnectionString = string.Empty;
        private readonly StorageAccountOptions _settings;
        private readonly CrypticPayUser _user;
        public string BlobURI { get; set; }
        public string URIBase = "https://crypticblob.blob.core.windows.net/";


        public BlobUtility(StorageAccountOptions settings, CrypticPayUser user)
        {
            _settings = settings;
            _user = user;
            ConnectionString = _settings.StorageAccountConnectionString;
        }

        // update to handle scaling
        public async Task UploadImage(Stream fileStream, string fileName)
        {
            var containerName = _settings.FullImagesContainerNameOption;
            // Create a BlobServiceClient object which will be used to create a container client
            BlobServiceClient blobServiceClient = new BlobServiceClient(ConnectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            // Get a reference to a blob
            string blobName = GenerateFileName(fileName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(fileStream, true);
            BlobURI = URIBase + containerName + "/" + blobName;
        }

        // generate file name for blob object
        private string GenerateFileName(string fileName)
        {
            string strFileName = string.Empty;
            string[] strName = fileName.Split('.');
           
            strFileName = _user.Id + "/" + DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd") + "/" + DateTime.Now.ToUniversalTime().ToString("yyyyMMdd\\THHmmssfff") + "/"+ fileName;
            return strFileName;
        }

        public string GetBlobURI(bool isScaled=true)
        {   
            return BlobURI;
        }

    }
}
