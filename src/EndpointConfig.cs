namespace ASB.NativeIntegration
{
    using NServiceBus;

    /*
		This class configures this endpoint as a Server. More information about how to configure the NServiceBus host
		can be found here: http://particular.net/articles/the-nservicebus-host
	*/
    public class EndpointConfig : IConfigureThisEndpoint
    {
        public void Customize(BusConfiguration configuration)
        {
            // Step 1: swap out the dequeue strategy by one that doesn't assume the brokered message content to be a byte[]
            
            // looks like most containers do not support removal of already registered types
            // so the trick is to prevent the registration of the original dequeuestrategy to begin with
            // I used a wrapper around the ninject container, but the same pattern should work with all
            configuration.UseContainer<PreventRegistrationBuilder>();
            
            // now we register our own dequeue strategy, this is just a copy of the original and some of it's code dependencies
            // there is only 1 change in BrokeredMessageConverter.ToTransportMessage where the body is consumed as a string 
            // instead of a byte[] depending on your scenario, this may be a stream as well
            configuration.RegisterComponents(r => r.ConfigureComponent<RawAzureServiceBusDequeueStrategy>(DependencyLifecycle.InstancePerCall));
            
            // Step 2: tell the endpoint to listen on a specific queue

            // configure the name of the native queue (als has impact on publishing registration, see MessageEndpointsMappings of DownstreamEndpoint)
            configuration.EndpointName("nativequeue");
            // prevents the inclusion of the machine name in the queue name, as is the default for this host, not needed for azure hosts
            configuration.ScaleOut().UseSingleBrokerQueue();

            // Step 3: swap out the deserialization logic
            // basically a copy of the original deserialization behavior but with one change
            //  if (transportMessage.IsNativeMessage())
            //  {
            //      ExtractNative(context, transportMessage);
            //  }
            //  else the usual path
            configuration.EnableFeature<CustomDeserializerFeature>();

            // the usual stuff
            configuration.UsePersistence<InMemoryPersistence>();
            configuration.UseTransport<AzureServiceBusTransport>();
            
        }
        
    }
}
