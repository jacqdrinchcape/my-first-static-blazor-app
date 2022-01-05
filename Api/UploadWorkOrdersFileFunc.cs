using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage.Blobs;

namespace BlazorApp.Api
{
    public static class UploadWorkOrdersFileFunc
    {
        [FunctionName("UploadWorkOrdersFileFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                var container = GetContainer();
                string fileName = $"{Guid.NewGuid().ToString()}.csv";
                var blob = container.GetBlobClient(fileName);
                await blob.UploadAsync(req.Body);
            }
            catch (Exception)
            {
                return new BadRequestObjectResult("Error uploading file");
            }

            return new OkObjectResult("ok");
        }

        #region helper
        private static BlobContainerClient GetContainer()
        {
            var blobConnectionString = Environment.GetEnvironmentVariable("blobConnectionString");
            var blobworkordersStorageContainer = Environment.GetEnvironmentVariable("blobworkordersStorageContainer");
            var container = new BlobContainerClient(blobConnectionString, blobworkordersStorageContainer);
            container.CreateIfNotExists();

            return container;
        }
        #endregion
    }
}
