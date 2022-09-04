using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace LTuri.Abp.Application.Events.EventBus
{
    /// <summary>
    /// TODO: docs
    /// </summary>
    public class AggregatedEventBus : ITransientDependency , IEventBus
    {
        private readonly IEnumerable<IEventBus> eventBuses;

        public AggregatedEventBus(
            List<IEventBus> eventBuses
        ) {
            this.eventBuses = eventBuses;
        }

        public async Task PublishAsync(Type eventType, object eventData, bool onUnitOfWorkComplete = true)
        {
            foreach (var eventBus in eventBuses)
            {
                await eventBus.PublishAsync(eventType, eventData, onUnitOfWorkComplete);
            }
        }

        public async Task PublishAsync<TEvent>(TEvent eventData, bool onUnitOfWorkComplete = true) where TEvent : class
        {
            foreach (var eventBus in eventBuses)
            {
                await eventBus.PublishAsync(eventData, onUnitOfWorkComplete);
            }
        }

        public IDisposable Subscribe<TEvent>(Func<TEvent, Task> action) where TEvent : class
        {
            return new AggregatedDisposable(eventBuses.Select(
                eventBus => eventBus.Subscribe(action)
            ).ToArray());
        }

        public IDisposable Subscribe<TEvent, THandler>()
            where TEvent : class
            where THandler : IEventHandler, new()
        {
            return new AggregatedDisposable(eventBuses.Select(
                eventBus => eventBus.Subscribe<TEvent, THandler>()
            ).ToArray());
        }

        public IDisposable Subscribe(Type eventType, IEventHandler handler)
        {
            return new AggregatedDisposable(eventBuses.Select(
                eventBus => eventBus.Subscribe(eventType, handler)
            ).ToArray());
        }

        public IDisposable Subscribe<TEvent>(IEventHandlerFactory factory) where TEvent : class
        {
            return new AggregatedDisposable(eventBuses.Select(
                eventBus => eventBus.Subscribe<TEvent>(factory)
            ).ToArray());
        }

        public IDisposable Subscribe(Type eventType, IEventHandlerFactory factory)
        {
            return new AggregatedDisposable(eventBuses.Select(
                eventBus => eventBus.Subscribe(eventType, factory)
            ).ToArray());
        }

        public void Unsubscribe<TEvent>(Func<TEvent, Task> action) where TEvent : class
        {
            foreach (var eventBus in eventBuses)
            {
                eventBus.Unsubscribe(action);
            }
        }

        public void Unsubscribe<TEvent>(ILocalEventHandler<TEvent> handler) where TEvent : class
        {
            foreach (var eventBus in eventBuses)
            {
                eventBus.Unsubscribe(handler);
            }
        }

        public void Unsubscribe(Type eventType, IEventHandler handler)
        {
            foreach (var eventBus in eventBuses)
            {
                eventBus.Unsubscribe(eventType, handler);
            }
        }

        public void Unsubscribe<TEvent>(IEventHandlerFactory factory) where TEvent : class
        {
            foreach (var eventBus in eventBuses)
            {
                eventBus.Unsubscribe<TEvent>(factory);
            }
        }

        public void Unsubscribe(Type eventType, IEventHandlerFactory factory)
        {
            foreach (var eventBus in eventBuses)
            {
                eventBus.Unsubscribe(eventType, factory);
            }
        }

        public void UnsubscribeAll<TEvent>() where TEvent : class
        {
            foreach (var eventBus in eventBuses)
            {
                eventBus.UnsubscribeAll<TEvent>();
            }
        }

        public void UnsubscribeAll(Type eventType)
        {
            foreach (var eventBus in eventBuses)
            {
                eventBus.UnsubscribeAll(eventType);
            }
        }
    }
}
