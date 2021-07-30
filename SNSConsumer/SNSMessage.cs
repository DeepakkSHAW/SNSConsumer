using System;
using System.Collections.Generic;
using System.Text;

namespace SNSConsumer
{
    public class SNSMessage
    {
        public string Type { get; set; }
        public string MessageId { get; set; }
        public string TopicArn { get; set; }
        public string Token { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public string SignatureVersion { get; set; }
        public string Signature { get; set; }
        public string SigningCertURL { get; set; }
        public string SubscribeURL { get; set; }
        public string UnsubscribeURL { get; set; }
    }
}
