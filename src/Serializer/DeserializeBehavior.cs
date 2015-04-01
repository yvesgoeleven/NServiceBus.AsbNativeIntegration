using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ASB.NativeIntegration.Messages;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Pipeline;
using NServiceBus.Pipeline.Contexts;
using NServiceBus.Serialization;
using NServiceBus.Unicast.Messages;

namespace ASB.NativeIntegration
{
    class DeserializeBehavior : IBehavior<IncomingContext>
    {
        public IMessageSerializer MessageSerializer { get; set; }
        
        public LogicalMessageFactory LogicalMessageFactory { get; set; }

        public MessageMetadataRegistry MessageMetadataRegistry { get; set; }

        public void Invoke(IncomingContext context, Action next)
        {
            var transportMessage = context.PhysicalMessage;

            if (transportMessage.IsControlMessage())
            {
                log.Info("Received a control message. Skipping deserialization as control message data is contained in the header.");
                next();
                return;
            }

            if (transportMessage.IsNativeMessage())
            {
                ExtractNative(context, transportMessage);
            }
            else
            {
                try
                {
                    context.LogicalMessages = Extract(transportMessage);
                }
                catch (Exception exception)
                {
                    throw new MessageDeserializationException(transportMessage.Id, exception);
                }
            }
            

            next();
        }

        private void ExtractNative(IncomingContext context, TransportMessage transportMessage)
        {
            //assuming all inbound native messages are of the same type
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof (TestCommand));
            var memStream = new MemoryStream(transportMessage.Body);
            var resultingMessage = serializer.Deserialize(memStream);
            var logicalMessage = LogicalMessageFactory.Create(typeof (TestCommand), resultingMessage,
                transportMessage.Headers);
            context.LogicalMessages = new List<LogicalMessage> {logicalMessage};
        }

        List<LogicalMessage> Extract(TransportMessage physicalMessage)
        {
            if (physicalMessage.Body == null || physicalMessage.Body.Length == 0)
            {
                return new List<LogicalMessage>();
            }

            string messageTypeIdentifier;
            var messageMetadata = new List<MessageMetadata>();

            if (physicalMessage.Headers.TryGetValue(Headers.EnclosedMessageTypes, out messageTypeIdentifier))
            {
                foreach (var messageTypeString in messageTypeIdentifier.Split(';'))
                {
                    var typeString = messageTypeString;

                    var metadata = MessageMetadataRegistry.GetMessageMetadata(typeString);
                    if (metadata == null)
                    {
                        continue;
                    }
                    messageMetadata.Add(metadata);
                }

                if (messageMetadata.Count == 0 && physicalMessage.MessageIntent != MessageIntentEnum.Publish)
                {
                    log.WarnFormat("Could not determine message type from message header '{0}'. MessageId: {1}", messageTypeIdentifier, physicalMessage.Id);
                }
            }

            using (var stream = new MemoryStream(physicalMessage.Body))
            {
                var messageTypesToDeserialize = messageMetadata.Select(metadata => metadata.MessageType).ToList();
                return MessageSerializer.Deserialize(stream, messageTypesToDeserialize)
                    .Select(x => LogicalMessageFactory.Create(x.GetType(), x, physicalMessage.Headers))
                    .ToList();

            }
        }
        
        static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}