using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Microsoft.WindowsAzure.Storage.Blob;

namespace FunctionBlob
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task Run(
            [BlobTrigger("images/{name}", Connection = "hellostorage")]
            CloudBlockBlob blob, string name, ILogger log,
            [CosmosDB("cosmossrv90", "images", ConnectionStringSetting = "cosmoscs")]
            IAsyncCollector<FaceAnalysisResults> result)
            
           
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{blob.Name} \n Size: {blob.Properties.Length} Bytes");
            var sas = GetSas(blob);
            var url = blob.Uri + sas;
            log.LogInformation($"Blob url is {url}");

            var faces = await GetAnalysisAsync(url);
            await result.AddAsync(new FaceAnalysisResults { Faces = faces, ImageId = blob.Name });
        }

        public static async Task<Face[]> GetAnalysisAsync(String url)
        {
            var client = new FaceServiceClient("046b585dd8724c3bbebef5eab76aed03", "https://faceapi90.cognitiveservices.azure.com/face/v1.0");
            var types = new[] { FaceAttributeType.Emotion };
            var result = await client.DetectAsync(url, false, false, types);
            return result;
        }



        public static string GetSas(CloudBlockBlob blob)
        {
            var sasPolicy = new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-15),
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(15)
            };

            var sas = blob.GetSharedAccessSignature(sasPolicy);
            return sas;
        }
    }
}
