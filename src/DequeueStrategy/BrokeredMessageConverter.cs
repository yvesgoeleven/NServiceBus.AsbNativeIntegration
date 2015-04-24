using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ServiceBus.Messaging;
using NServiceBus;

namespace ASB.NativeIntegration
{
    public static class BrokeredMessageConverter
    {
        public static TransportMessage ToTransportMessage(this BrokeredMessage message)
        {
            TransportMessage t;
            var rawMessage = message.GetBody<string>() ?? "";

            if (message.Properties.Count > 0)
            {
                var headers = message.Properties.ToDictionary(kvp => kvp.Key, kvp => kvp.Value as string);
                if (!String.IsNullOrWhiteSpace(message.ReplyTo))
                {
                    headers[Headers.ReplyToAddress] = message.ReplyTo;
                }

                t = new TransportMessage(message.MessageId, headers)
                {
                    CorrelationId = message.CorrelationId,
                    TimeToBeReceived = message.TimeToLive,
                    MessageIntent = (MessageIntentEnum)Enum.Parse(typeof(MessageIntentEnum), message.Properties[Headers.MessageIntent].ToString()),
                    Body = Encoding.UTF8.GetBytes(rawMessage)
                };
            }
            else
            {
                t = new TransportMessage(message.MessageId, new Dictionary<string, string>())
                {
                    Body = Encoding.UTF8.GetBytes(rawMessage)
                };
            }

            return t;
        }
    }
}