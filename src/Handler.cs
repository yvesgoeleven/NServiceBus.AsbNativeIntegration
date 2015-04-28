using System;
using ASB.NativeIntegration.Messages;
using NServiceBus;

namespace ASB.NativeIntegration
{
    public class Handler : IHandleMessages<TestCommand>
    {
        public IBus Bus { get; set; }

        public void Handle(TestCommand message)
        {
            Console.WriteLine("Received test command");

            // uncomment to test FLR
             throw new Exception("some exception");

            Bus.Publish(new TestEvent{ SomeContent = message.SomeContent });
        }
    }
}
