using LTuri.Abp.Application.EntityFramework;
using LTuri.Abp.Application.EntityFramework.Enum;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus;

namespace LTuri.Abp.Application.Events.EventBus
{
    public class WebhookEventBus : ITransientDependency, IEventBus
    {
        private readonly IRepository<EventEntity> repository;

        public WebhookEventBus(
            IRepository<EventEntity> repository
        )
        {
            this.repository = repository;
        }

        /// <summary>
        /// Compared to the normal EventBus, this eventBus published only events
        /// that implements AbstractEvent and have an attribute EventAttribute.
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventData"></param>
        /// <param name="onUnitOfWorkComplete"></param>
        /// <returns></returns>
        public async Task PublishAsync<TEvent>(TEvent eventData, bool onUnitOfWorkComplete = true) where TEvent : class
        {
            var eventAttribute = ExtractEventAttribute(eventData);
            var abstractEvent = ConvertEventData(eventData);

            if (eventAttribute != null && abstractEvent != null)
            {
                var webhookQueue = await repository.FirstOrDefaultAsync(x =>
                    x.EventName == eventAttribute.EventName &&
                    x.EntityName == eventAttribute.EntityName &&
                    x.EntityId == abstractEvent.EntityId &&
                    x.EventStatus == EventStatus.Pending
                );

                if (webhookQueue == null)
                {
                    // Create
                    webhookQueue = new EventEntity()
                    {
                        EventName = eventAttribute.EventName,
                        EntityName = eventAttribute.EntityName,
                        EntityId = abstractEvent.EntityId,
                        Changed = abstractEvent.Changed
                    };
                    await repository.InsertAsync(webhookQueue);
                }
                else
                {
                    // Compute unique changed parameters
                    var changedUnion = new List<string>();
                    changedUnion.AddRange(abstractEvent.Changed);
                    changedUnion.AddRange(webhookQueue.Changed);
                    webhookQueue.Changed = changedUnion.ToHashSet().ToArray();
                    await repository.UpdateAsync(webhookQueue);
                }
            }
        }

        public Task PublishAsync(Type eventType, object eventData, bool onUnitOfWorkComplete = true)
        {
            return PublishAsync(eventData, onUnitOfWorkComplete);
        }

        protected EventAttribute? ExtractEventAttribute<TEvent>(TEvent eventData) 
            where TEvent : class
        {
            var attributes = eventData.GetType().GetCustomAttributes(typeof(EventAttribute), true);
            var attribute = attributes.FirstOrDefault();
            if (attribute != null)
            {
                return (EventAttribute)attribute;
            }
            return null;
        }
        protected AbstractEvent? ConvertEventData<TEvent>(TEvent eventData)
        {
            if (eventData is AbstractEvent abstractEventData)
            {
                return abstractEventData;
            }
            return null;
        }

        #region Unimplemented methods
        // The methods in this region were intentionally not implemented:
        // it trully implement the Subscribe/Unsubscribe/UnsubscribeAll, the developer
        // needs to explicitely call the proper methods in the repository.
        // Declaring them here might cause loss of data
        public IDisposable Subscribe<TEvent>(Func<TEvent, Task> action) where TEvent : class
        {
            return new WebhookDisposable();
        }

        public IDisposable Subscribe<TEvent, THandler>()
            where TEvent : class
            where THandler : IEventHandler, new()
        {
            return new WebhookDisposable();
        }

        public IDisposable Subscribe(Type eventType, IEventHandler handler)
        {
            return new WebhookDisposable();
        }

        public IDisposable Subscribe<TEvent>(IEventHandlerFactory factory) where TEvent : class
        {
            return new WebhookDisposable();
        }

        public IDisposable Subscribe(Type eventType, IEventHandlerFactory factory)
        {
            return new WebhookDisposable();
        }

        public void Unsubscribe<TEvent>(Func<TEvent, Task> action) where TEvent : class
        {
            // Method intentionally left empty.
        }

        public void Unsubscribe<TEvent>(ILocalEventHandler<TEvent> handler) where TEvent : class
        {
            // Method intentionally left empty.
        }

        public void Unsubscribe(Type eventType, IEventHandler handler)
        {
            // Method intentionally left empty.
        }

        public void Unsubscribe<TEvent>(IEventHandlerFactory factory) where TEvent : class
        {
            // Method intentionally left empty.
        }

        public void Unsubscribe(Type eventType, IEventHandlerFactory factory)
        {
            // Method intentionally left empty.
        }

        public void UnsubscribeAll<TEvent>() where TEvent : class
        {
            // Method intentionally left empty.
        }

        public void UnsubscribeAll(Type eventType)
        {
            // Method intentionally left empty.
        }
        #endregion
    }
}
