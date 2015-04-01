using NServiceBus;

namespace ASB.NativeIntegration.Messages
{
    public class TestCommand : ICommand
    {
        public string SomeContent { get; set; }
    }

    public class TestEvent : IEvent
    {
        public string SomeContent { get; set; }
    }
}
