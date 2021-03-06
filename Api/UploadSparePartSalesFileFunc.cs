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
    public static class UploadSparePartSalesFileFunc
    {
        [FunctionName("UploadSparePartSalesFileFunc")]
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

            //string responseMessage = string.IsNullOrEmpty(name)
            //    ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            //    : $"Hello, {name}. This HTTP triggered function executed successfully.";

            //return new OkObjectResult(responseMessage);
        }

        #region helper
        private static BlobContainerClient GetContainer()
        {
            var blobConnectionString = Environment.GetEnvironmentVariable("blobConnectionString");
            var blobsparepartsalesStorageContainer = Environment.GetEnvironmentVariable("blobsparepartsalesStorageContainer");
            var container = new BlobContainerClient(blobConnectionString, blobsparepartsalesStorageContainer);
            container.CreateIfNotExists();

            return container;
        }
        #endregion
    }
}
