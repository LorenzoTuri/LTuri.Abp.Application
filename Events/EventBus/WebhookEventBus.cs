using LTuri.Abp.Application.Events.Enum;
using LTuri.Abp.Application.Events;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace LTuri.Abp.Application.Events.EventBus
{
    public class WebhookEventBus<TEntity> : ITransientDependency
        where TEntity : class, IWebhookQueueEntity, IEntity, new()
    {
        private readonly IRepository<TEntity> repository;

        public WebhookEventBus(
            IRepository<TEntity> repository
        )
        {
            this.repository = repository;
        }

        public async Task PublishAsync<TEvent>(EventAttribute eventAttribute, TEvent eventData) where TEvent : AbstractEvent
        {
            var webhookQueue = await repository.FirstOrDefaultAsync(x =>
                x.EventName == eventAttribute.EventName &&
                x.EntityName == eventAttribute.EntityName &&
                x.EntityId == eventData.EntityId &&
                x.EventStatus == WebhookQueueStatuses.Pending
            );
            
            if (webhookQueue == null)
            {
                // Create
                webhookQueue = new TEntity()
                {
                    EventName = eventAttribute.EventName,
                    EntityName = eventAttribute.EntityName,
                    EntityId = eventData.EntityId,
                    Changed = eventData.Changed
                };
                await repository.InsertAsync(webhookQueue);
            }
            else
            {
                // Compute unique changed parameters
                var changedUnion = new List<string>();
                changedUnion.AddRange(eventData.Changed);
                changedUnion.AddRange(webhookQueue.Changed);
                webhookQueue.Changed = changedUnion.ToHashSet().ToArray();
                await repository.UpdateAsync(webhookQueue);
            }
        }
    }
}
