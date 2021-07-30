using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace SNSConsumer
{
    public class FnCreateOrderToSqure
    {
        private readonly HttpClient _http;
        private readonly IHttpClientFactory _clientFactory;
        public FnCreateOrderToSqure(HttpClient httpClient, IHttpClientFactory clientFactory)
        {
            _http = httpClient;
            _clientFactory = clientFactory;
        }

        protected async Task<string> Send2Square(string msg)
        {
            try
            {
                StringContent httpContent = new StringContent(msg, Encoding.UTF8, "application/json");

                //_http.DefaultRequestHeaders.Add("Authorization", "Bearer EAAAEFiWJgCoujGlvyzOBjGjCMm_KMORSeVdR-IFA3cSR5zIehCZltqmLXQKYCLl");
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("SquareUpURLToken"));

                _http.DefaultRequestHeaders.Add("Square-Version", "2021-06-16");

                var response = await _http.PostAsync("https://connect.squareupsandbox.com/v2/orders", httpContent);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        protected async Task<string> Send2SquareUp(string msg)
        {
            try
            {
                StringContent httpContent = new StringContent(msg, Encoding.UTF8, "application/json");

                var client = _clientFactory.CreateClient("SquareUp");
                var response = await client.PostAsync("orders", httpContent);

                //if (response.IsSuccessStatusCode)
                //{
                //    using var responseStream = await response.Content.ReadAsStreamAsync();
                //    dynamic branches = await JsonSerializer.DeserializeAsync<IEnumerable<dynamic>>(responseStream);
                //}
                //else
                //{
                //    // Error Handling
                //}

                return await response.Content.ReadAsStringAsync();

            }
            catch (Exception)
            {
                throw;
            }
        }

        //*******************Activate Subscription from AWS SNS**************//
        protected async Task<bool> ActivateSubscription(string msg)
        {
            try
            {
                SNSMessage msgSNS = JsonSerializer.Deserialize<SNSMessage>(msg);

                HttpResponseMessage response = await _http.GetAsync(msgSNS.SubscribeURL);
                //response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseBody);
                    return true;
                }
                else
                { return false; }
                
                
            }
            catch (Exception)
            {
                throw;
            }

        }
        [FunctionName("FnCreateOrderToSqure")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var resposeAPI = string.Empty;
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                SNSMessage msg = JsonSerializer.Deserialize<SNSMessage>(requestBody);

                if (string.IsNullOrEmpty(msg.Type)) { log.LogError("Missing TYPE from incoming message"); return new BadRequestResult(); }
                else
                {
                    if (msg.Type.Contains("Notification"))
                    {
                        if (string.IsNullOrEmpty(msg.Message)) { log.LogError("Missing Message from incoming message"); return new BadRequestResult(); }
                        else
                        {
                            //Sq msgSquere = msg.Message;
                            OrderFromSNS orderFromSNS = System.Text.Json.JsonSerializer.Deserialize<OrderFromSNS>(msg.Message);
                            var POS = Environment.GetEnvironmentVariable("Order2POS");

                            if (POS.Split(new char[] { ',', ';' }).ToList().Contains(orderFromSNS.store_id.ToString()))
                            {
                                log.LogWarning("Message has been sent to POS");
                                resposeAPI = $"Store ID {orderFromSNS.store_id}, sent to POS";
                            }
                            else
                            {
                                orderFromSNS.idempotency_key = Guid.NewGuid().ToString();
                                orderFromSNS.order.location_id = "LF2WSFA3RYR3J";

                                //resposeSquareUp = await Send2Square(JsonSerializer.Serialize(orderFromSNS));
                                resposeAPI = await Send2SquareUp(JsonSerializer.Serialize(orderFromSNS));

                                log.LogWarning("Message has been sent to SqureUp");
                            }
                        }

                    }
                    else if (msg.Type.Contains("SubscriptionConfirmation"))
                    {
                        log.LogWarning("SNS request for subscription");
                        log.LogWarning(requestBody);
                        var r = await ActivateSubscription(requestBody);
                    }
                    else
                    {
                        log.LogWarning("No Action taken");
                        log.LogWarning(requestBody);
                        resposeAPI = requestBody;
                    }
                }
                return new OkObjectResult($"{resposeAPI}");
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                throw;
            }
        }
    }
}
