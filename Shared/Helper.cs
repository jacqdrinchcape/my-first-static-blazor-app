using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BlazorApp.Shared
{
    public class Helper
    {
        public enum DocumentType
        {
            VehicleSales = 1,
            WorkOrders = 2,
            SparePartSales = 3,
            WorkOrderSpareParts = 4,
            SparePartStock = 5
        }

        public static BlobContainerClient GetContainer(string blobContainer)
        {
            var blobConnectionString = Environment.GetEnvironmentVariable("blobConnectionString");
            var blobStorageContainer = Environment.GetEnvironmentVariable(blobContainer);
            var container = new BlobContainerClient(blobConnectionString, blobStorageContainer);
            container.CreateIfNotExists();

            return container;
        }

        public static async Task<int>  VerifyDocument(Stream body, string documenttype)
        {
            int resp = 0;
            if (body.Length == 0)
            {
                //Empty File
                return 1;
            }

            var reader = new StreamReader(body);
            var lineNumber = 1;
            var line = await reader.ReadLineAsync();
            while (lineNumber <= 2)
            {
                //Verify document_type - first column
                if (lineNumber == 2)
                {
                    line = await reader.ReadLineAsync();

                    //Empty line
                    if (line == null)
                    {
                        return 2;
                    }
                    var parts = line.Split(',');

                    //Invalid Document Type
                    if (parts[0] != documenttype)
                    {
                        return 3;
                    }

                }
                lineNumber++;
            }

            return resp;
        }

        public static string ErrorMessage(int errorCode)
        {
            string errorMessage = string.Empty;
            switch (errorCode)
            {
                case 1:
                    errorMessage = "Empty File...";
                    break;

                case 2:
                    errorMessage = "Empty Line...";
                    break;

                case 3:
                    errorMessage = "Invalid Document Type...";
                    break;

                default: errorMessage = "Error";
                    break;
            }
               
            return errorMessage;

        }

    }
}
