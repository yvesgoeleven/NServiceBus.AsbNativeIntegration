using System.Text;
using NServiceBus;
using NServiceBus.Unicast.Messages;

namespace ASB.NativeIntegration
{
    static class TransportMessageExtensions
    {
        public static bool IsNativeMessage(this TransportMessage transportMessage)
        {
            // assumption that content is an utf8 encoded string
            var decoded = Encoding.UTF8.GetString(transportMessage.Body);
            return decoded.StartsWith("<");
        }

        public static bool IsControlMessage(this TransportMessage transportMessage)
        {
            return transportMessage.Headers != null &&
                   transportMessage.Headers.ContainsKey(Headers.ControlMessageHeader);
        }


        public static bool IsControlMessage(this LogicalMessage transportMessage)
        {
            return transportMessage.Headers != null &&
                   transportMessage.Headers.ContainsKey(Headers.ControlMessageHeader);
        }
    }
}