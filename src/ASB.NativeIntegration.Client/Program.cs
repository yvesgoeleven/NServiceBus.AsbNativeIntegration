using System;
using System.Text;
using Microsoft.ServiceBus.Messaging;

namespace ASB.NativeIntegration.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = MessagingFactory.CreateFromConnectionString("enter connectionstring");
            var client = factory.CreateMessageSender("nativequeue");
            var body = "<TestCommand><SomeContent>Some Data</SomeContent></TestCommand>";

            Console.WriteLine("Hit any key to send a native message, or x to quit");
            var x = Console.ReadLine();

            while (x != "x")
            {
                client.Send(new BrokeredMessage(body)
                {
                    MessageId = Guid.NewGuid().ToString(),
                    ContentType = "text/xml"
                });

                Console.WriteLine("Hit any key to send a native message, or x to quit");
                x = Console.ReadLine();
            }
        }
    }
}
