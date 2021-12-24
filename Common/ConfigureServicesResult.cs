using System;
using MassTransit.ExtensionsDependencyInjectionIntegration;

namespace Common
{
    public record ConfigureServicesResult
    {
        public bool AddMassTransit { get; }
        public Action<IServiceCollectionBusConfigurator> MassTransitCallback { get; }
        
        private ConfigureServicesResult()
        {
        }

        private ConfigureServicesResult(Action<IServiceCollectionBusConfigurator> massTransitCallback)
        {
            MassTransitCallback = massTransitCallback;
            AddMassTransit = true;
        }

        public static ConfigureServicesResult Empty() => new ConfigureServicesResult();
        public static ConfigureServicesResult MassTransit(Action<IServiceCollectionBusConfigurator> callback) => new ConfigureServicesResult(callback);
    }
}