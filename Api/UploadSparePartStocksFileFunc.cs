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
using Shared = BlazorApp.Shared;

namespace BlazorApp.Api
{
    public static class UploadSparePartStocksFileFunc
    {
        [FunctionName("UploadSparePartStocksFileFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "UploadSparePartStocksFileFunc")] HttpRequest req,
            ILogger log)
        {
            
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                int isValidDocument = await Shared.Helper.VerifyDocument(req.Body, "documenttypesparepartstock");
                
                if (isValidDocument == 0)
                {
                    var container = Shared.Helper.GetContainer("blobsparepartstockStorageContainer");
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
        //private static BlobContainerClient GetContainer()
        //{
        //    var blobConnectionString = Environment.GetEnvironmentVariable("blobConnectionString");
        //    var blobsparepartstockStorageContainer = Environment.GetEnvironmentVariable("blobsparepartstockStorageContainer");
        //    var container = new BlobContainerClient(blobConnectionString, blobsparepartstockStorageContainer);
        //    container.CreateIfNotExists();

        //    return container;
        //}

        //private static int VerifyDocument(Stream body, string documenttypesparepartstock)
        //{
        //    int resp = 0;

        //    var reader = new StreamReader(body);
        //    var lineNumber = 1;
        //    var line = reader.ReadLine();
        //    while (lineNumber <= 2)
        //    {
        //        //Verify document_type - first column
        //        if (lineNumber == 2)
        //        {
        //            line = reader.ReadLine();

        //            //Empty file
        //            if (line == null)
        //            {
        //                return 1;
        //            }
        //            var parts = line.Split(',');

        //            if (parts[0] != Environment.GetEnvironmentVariable("documenttypesparepartstock"))
        //            {
        //               return 1;
        //            }
                    
        //        }
        //        lineNumber++;
        //    }

        //    return resp;
        //}
        #endregion
    }
}
