using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization.EventEmitters;

namespace YamlJsonFunctions
{
    public static class ConvertYamlToJson
    {
        [FunctionName("ConvertYamlToJson")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest request,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //var yamlStream = new YamlStream();
            using (var reader = new StreamReader(request.Body))
            {
                string s = await reader.ReadToEndAsync();

                //    yamlStream.Load(reader);
                var deserializer = new YamlDotNet.Serialization.DeserializerBuilder().Build();
                var obj = deserializer.Deserialize<object>(s);

                var serializer = new YamlDotNet.Serialization.SerializerBuilder().JsonCompatible().Build();

                return new HttpResponseMessage
                {
                    Content = new PushStreamContent((stream, content, context) =>
                    {
                        using (var writer = new StreamWriter(stream))
                            serializer.Serialize(writer, obj);
                    }),
                };
            }

            //return name != null
            //    ? (ActionResult)new OkObjectResult($"Hello, {name}")
            //    : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
