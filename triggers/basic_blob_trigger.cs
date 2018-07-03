using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.AspNetCore.Http;
using Clarifai.API;
using Clarifai.DTOs.Inputs;
using Clarifai.DTOs.Searches;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace triggers
{
    public static class basic_blob_trigger
    {
        [FunctionName("basic_blob_trigger")]
        public static async Task Run(
            [BlobTrigger("test/{name}", Connection = "AzureWebJobsBlob")]Stream myBlob,
            string name,
            TraceWriter log)
        {
            log.Info($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            var runClarifai = false;
            if (runClarifai) {
                var client = new ClarifaiClient("");
                var jsonResponse = await client.PublicModels.GeneralModel
                    .Predict(new ClarifaiURLImage($"https://storagesample.blob.core.windows.net/test/{name}"))
                    .ExecuteAsync();

                var token = JToken.Parse(jsonResponse.RawBody);
                var data = token.SelectTokens("outputs[0].data.concepts[*].name");
                foreach (JToken item in data)
                    log.Info(item.ToString());
            }
        }
    }
}
