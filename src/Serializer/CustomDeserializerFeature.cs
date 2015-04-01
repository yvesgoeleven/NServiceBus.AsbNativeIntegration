using System;
using System.Linq;
using Ninject;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.Pipeline;
using NServiceBus.Transports;
using NServiceBus.Unicast.Transport;

namespace ASB.NativeIntegration
{
    public class CustomDeserializerFeature : Feature
    {
        public CustomDeserializerFeature()
        {
            EnableByDefault();
        }

        protected override void Setup(FeatureConfigurationContext context)
        {
            context.Pipeline.Replace(WellKnownStep.DeserializeMessages, typeof(DeserializeBehavior));
        }

    }
}