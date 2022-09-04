using LTuri.Abp.Application.Events.EventBus;
using Microsoft.Extensions.DependencyInjection;
using LTuri.Abp.Application.Services.Options;

namespace LTuri.Abp.Application.Services
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddApplicationEventBuses(
            this IServiceCollection services,
            ApplicationEventBusOptions? options = null
        )
        {
            var configuration = options != null ? options : new ApplicationEventBusOptions();

            services.AddTransient(serviceProvider =>
            {
                return new AggregatedEventBus(
                    configuration.GetEventBuses(serviceProvider)
                );
            });

            return services;
        }
    }
}