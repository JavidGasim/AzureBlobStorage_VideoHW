﻿using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureBlobStorage_VideoHW.Services.Abstracts;

namespace AzureBlobStorage_VideoHW.Services.Concretes
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobContainerClient _containerClient;
        public BlobStorageService(IConfiguration configuration)
        {
            var connectionString = configuration["AzureBlobStorage:ConnectionString"];
            var containerName = configuration["AzureBlobStorage:ContainerName"];
            _containerClient = new BlobContainerClient(connectionString, containerName);
        }

        public async Task<Stream> DownloadAsync(string fileName)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            var download = await blobClient.DownloadAsync();
            return download.Value.Content;
        }

        public async Task<List<string>> ListFilesAsync()
        {
            var blobUris = new List<string>();
            await foreach (var blobItem in _containerClient.GetBlobsAsync())
            {
                var blobClient = _containerClient.GetBlobClient(blobItem.Name);
                blobUris.Add(blobClient.Uri.ToString());
            }
            return blobUris;
        }

        public async Task<string> UploadAsync(Stream fileStream, string fileName)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);

            // Video dosyası olduğundan emin olmak için ContentType ayarlanıyor
            var headers = new BlobHttpHeaders
            {
                ContentType = "video/mp4" // MP4 formatında olduğunu varsayıyoruz
            };

            await blobClient.UploadAsync(fileStream, new BlobUploadOptions
            {
                HttpHeaders = headers
            });

            return blobClient.Uri.ToString();
        }
    }
}
