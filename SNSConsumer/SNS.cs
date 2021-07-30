using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SNSConsumer
{
    public class SNS
    {
        static async Task SendMessage(IAmazonSimpleNotificationService snsClient)
        {
            var request = new PublishRequest
            {
                TopicArn = "INSERT TOPIC ARN",
                Message = "Test Message"
            };

            await snsClient.PublishAsync(request);
        }

        internal static bool PublishMsg2SNS(string msg, string arn)
        {
            var vReturn = false;
            try
            {
                //var client = new AmazonSimpleNotificationServiceClient(Amazon.RegionEndpoint.USEast2);
                //SendMessage(client).Wait();

                var snsClient = new AmazonSimpleNotificationServiceClient(Amazon.RegionEndpoint.APSoutheast2);
                snsClient.PublishAsync(new PublishRequest
                {
                    Subject = $"From Azure Function {DateTime.Today.ToShortDateString()}",
                    Message = msg, //"Testing",
                    TopicArn = arn//"arn:aws:sns:ap-southeast-2:108653607457:dksquareup"
                }
                    ).Wait();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            return vReturn;
        }
    }
}
