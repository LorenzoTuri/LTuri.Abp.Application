using LTuri.Abp.Application.Events;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.EventBus;

namespace LTuri.Abp.Application.Events.EventBus
{
    public class AggregatedEventBus<TWebhookEventEntity> : ITransientDependency
        where TWebhookEventEntity : class, IWebhookQueueEntity, IEntity, new()
    {
        private readonly IEnumerable<IEventBus> eventBuses;
        private readonly WebhookEventBus<TWebhookEventEntity> webhookEventBus;

        public AggregatedEventBus(
            List<IEventBus> eventBuses, 
            WebhookEventBus<TWebhookEventEntity> webhookEventBus
        ) {
            this.eventBuses = eventBuses;
            this.webhookEventBus = webhookEventBus;
        }

        public async Task PublishAsync<TEvent>(TEvent eventData, bool onUnitOfWorkComplete = false)
            where TEvent : AbstractEvent
        {
            // Local events first
            foreach (var eventBus in eventBuses)
            {
                await eventBus.PublishAsync(eventData, onUnitOfWorkComplete);
            }
            // And webhook events
            await PublishWebhookAsync(eventData.GetType(), eventData);
        }

        public async Task PublishAsync(Type eventType, AbstractEvent eventData, bool onUnitOfWorkComplete = false)
        {
            // Local events first
            foreach (var eventBus in eventBuses)
            {
                await eventBus.PublishAsync(eventType, eventData, onUnitOfWorkComplete);
            }
            // And webhook events
            await PublishWebhookAsync(eventType, eventData);
        }

        private async Task PublishWebhookAsync(Type eventType, AbstractEvent eventData)
        {
            var attributes = eventType.GetCustomAttributes(typeof(EventAttribute), true);
            var attribute = attributes.FirstOrDefault();

            if (attribute != null)
            {
                await webhookEventBus.PublishAsync((EventAttribute)attribute, eventData);
            }
        }
    }
}
