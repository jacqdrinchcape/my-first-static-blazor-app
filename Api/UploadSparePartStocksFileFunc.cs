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
    public static class UploadSparePartStocksFileFunc
    {
        [FunctionName("UploadSparePartStocksFileFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "UploadSparePartStocksFileFunc")] HttpRequest req,
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
                return new BadRequestObjectResult("Error saving file");
            }

            return new OkObjectResult("ok");
        }

        #region helper
        private static BlobContainerClient GetContainer()
        {
            var blobConnectionString = Environment.GetEnvironmentVariable("blobConnectionString");
            var blobsparepartstockStorageContainer = Environment.GetEnvironmentVariable("blobsparepartstockStorageContainer");
            var container = new BlobContainerClient(blobConnectionString, blobsparepartstockStorageContainer);
            container.CreateIfNotExists();

            return container;
        }
        #endregion
    }
}
