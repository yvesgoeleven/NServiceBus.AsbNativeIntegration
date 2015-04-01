using System;
using System.Collections.Generic;
using Ninject;
using NServiceBus;
using NServiceBus.ObjectBuilder.Common;

namespace ASB.NativeIntegration
{
    public class PreventRegistrationObjectBuilder : IContainer
    {
        private IContainer _container;

        public PreventRegistrationObjectBuilder()
            : this(new StandardKernel())
        {
        }

        public PreventRegistrationObjectBuilder(IKernel existingKernel)
        {
            _container = (IContainer) Activator.CreateInstance(
                Type.GetType("NServiceBus.ObjectBuilder.Ninject.NinjectObjectBuilder, NServiceBus.ObjectBuilder.Ninject"), existingKernel);
        }

        public void Dispose()
        {
            _container.Dispose();
        }

        public object Build(Type typeToBuild)
        {
            return _container.Build(typeToBuild);
        }

        public IContainer BuildChildContainer()
        {
            return _container.BuildChildContainer();
        }

        public IEnumerable<object> BuildAll(Type typeToBuild)
        {
            return _container.BuildAll(typeToBuild);
        }

        public void Configure(Type component, DependencyLifecycle dependencyLifecycle)
        {
            //prevent azure servicebus dequeue strategy from registering
            if (component.FullName.EndsWith("NServiceBus.Azure.Transports.WindowsAzureServiceBus.AzureServiceBusDequeueStrategy"))
                return;

            _container.Configure(component, dependencyLifecycle);
        }

        public void Configure<T>(Func<T> component, DependencyLifecycle dependencyLifecycle)
        {
            _container.Configure(component, dependencyLifecycle);
        }

        public void ConfigureProperty(Type component, string property, object value)
        {
            _container.ConfigureProperty(component, property, value);
        }

        public void RegisterSingleton(Type lookupType, object instance)
        {
            _container.RegisterSingleton(lookupType, instance);
        }

        public bool HasComponent(Type componentType)
        {
            return _container.HasComponent(componentType);
        }

        public void Release(object instance)
        {
            _container.Release(instance);
        }
    }
}