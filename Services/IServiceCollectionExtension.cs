using LTuri.Abp.Application.Events.EventBus;
using LTuri.Abp.Application.Events;
using LTuri.Abp.Application.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace LTuri.Abp.Application.Services
{
    /// <summary>
    /// TODO: refactor -> this is not enough.
    /// Provide true entity and not only the IWebhookQueueEntity 
    /// Provide some way to use only local events
    /// Provide a service that automatically calls webhookEvents
    /// </summary>
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddWebhookEventBus<TWebhookQueueEntity>(
            this IServiceCollection services
        ) where TWebhookQueueEntity : class, IEntity, IWebhookQueueEntity, new()
        {
            services.TryAddTransient(serviceProvider => 
            {
                var webhookQueueEntityRepository = serviceProvider.GetService<IRepository<TWebhookQueueEntity>>();

                if (webhookQueueEntityRepository == null)
                    throw new ServiceNotFoundException(
                        "WebhookEventBus<" + typeof(IWebhookQueueEntity).Name + ">",
                        "IRepository<" + typeof(IWebhookQueueEntity).Name + ">"
                    );

                return new WebhookEventBus<TWebhookQueueEntity>(webhookQueueEntityRepository);
            });
            return services;
        }

        public static IServiceCollection AddAggregatedEventBus<TWebhookQueueEntity>(
            this IServiceCollection services,
            Func<IServiceProvider, List<Volo.Abp.EventBus.IEventBus>> callback
        ) where TWebhookQueueEntity : class, IEntity, IWebhookQueueEntity, new()
        {
            services.TryAddTransient(serviceProvider =>
            {
                var webhookEventBus = serviceProvider.GetService<WebhookEventBus<TWebhookQueueEntity>>();

                if (webhookEventBus == null)
                    throw new ServiceNotFoundException(
                        "AggregatedEventBus<" + typeof(IWebhookQueueEntity).Name + ">",
                        "WebhookEventBus<" + typeof(IWebhookQueueEntity).Name + ">"
                    );

                return new AggregatedEventBus<TWebhookQueueEntity>(
                    callback(serviceProvider),
                    webhookEventBus
                );
            });
            return services;
        }
    }
}