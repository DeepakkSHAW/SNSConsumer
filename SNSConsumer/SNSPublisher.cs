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
    public static class SNSPublisher
    {



        [FunctionName("SNSPublisher")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogWarning($"Started Time {DateTime.Now}");

            string responseMessage = string.Empty;
            try
            {
                var topicArn = Environment.GetEnvironmentVariable("TopicARN");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                //dynamic data = JsonConvert.DeserializeObject(requestBody);
                //payload = data.ToString();

                if (string.IsNullOrEmpty(requestBody))
                {
                    responseMessage = "Missing body, default message set to payload";
                    requestBody = "{\"store_id\":212,\"order\":{\"reference_id\":\"my-order-1558\",\"customer_id\":\"DIV003\",\"line_items\":[{\"quantity\":\"2\",\"base_price_money\":{\"amount\":2,\"currency\":\"AUD\"},\"name\":\"milk\",\"note\":\"hot milk only\"}]}}";
                }
                else
                {
                    responseMessage = requestBody;
                }

                SNS.PublishMsg2SNS(requestBody, topicArn);

                log.LogWarning($"End Time {DateTime.Now}");
                return new OkObjectResult(responseMessage);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                return new BadRequestObjectResult(ex);
            }

        }
    }
}
