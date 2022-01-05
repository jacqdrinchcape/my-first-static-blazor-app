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
    public static class UploadWorkOrderSparePartsFileFunc
    {
        [FunctionName("UploadWorkOrderSparePartsFileFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                int isValidDocument = await Shared.Helper.VerifyDocument(req.Body, Shared.Helper.DocumentType.WorkOrderSpareParts.ToString());

                if (isValidDocument == 0)
                {
                    var container = Shared.Helper.GetContainer("blobworkordersparepartsStorageContainer");
                    string fileName = $"{Guid.NewGuid().ToString()}.csv";
                    var blob = container.GetBlobClient(fileName);

                    req.Body.Position = 0;
                    await blob.UploadAsync(req.Body);
                }
                else
                {
                    return new BadRequestObjectResult(Shared.Helper.ErrorMessage(isValidDocument));
                }
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message.ToString());
            }

            return new OkObjectResult("ok");
        }

        #region helper
        private static BlobContainerClient GetContainer()
        {
            var blobConnectionString = Environment.GetEnvironmentVariable("blobConnectionString");
            var blobworkordersparepartsStorageContainer = Environment.GetEnvironmentVariable("blobworkordersparepartsStorageContainer");
            var container = new BlobContainerClient(blobConnectionString, blobworkordersparepartsStorageContainer);
            container.CreateIfNotExists();

            return container;
        }
        #endregion
    }
}
