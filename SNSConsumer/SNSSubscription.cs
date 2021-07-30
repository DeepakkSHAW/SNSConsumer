using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SNSConsumer
{
    public static class SNSSubscription
    {
        [FunctionName("SNSSubscription")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string responseMessage = "All is well";

            foreach (var q in req.Query)
            {
                log.LogWarning($"Query param Key {q.Key} has value {q.Value}"); 
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            string data = JsonConvert.DeserializeObject(requestBody).ToString();
            //name = name ?? data?.name;
            if (string.IsNullOrEmpty(data))
            {
                responseMessage = "No BODY";
            }
            else
            {
                responseMessage = data;
            }
            log.LogWarning($"BODY MSG :\n {responseMessage}");

                //? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                //: $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
