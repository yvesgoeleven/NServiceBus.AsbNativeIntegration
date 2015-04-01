using System;
using ASB.NativeIntegration.Messages;
using NServiceBus;

namespace ASB.NativeIntegration.DownstreamEndpoint
{
    public class DownstreamHandler : IHandleMessages<TestEvent>
    {
        public void Handle(TestEvent message)
        {
            Console.WriteLine("Test event with content '{0}' received", message.SomeContent);
        }
    }
}
