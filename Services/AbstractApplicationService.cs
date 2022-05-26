using LTuri.Abp.Application.EntityTypes;
using LTuri.Abp.Application.Events;
using LTuri.Abp.Application.Events.EventBus;
using LTuri.Abp.Application.Repositories.Criteria;
using LTuri.Abp.Application.Services;
using LTuri.Abp.Application.Services.Response;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.ObjectMapping;

namespace LTuri.Abp.Application.Services
{
    public abstract class AbstractApplicationService<
        // Entity config
        TEntity,
        // Output config
        TGetOutputDto,
        TListOutputDto,
        // Input config
        TCreateInputDto,
        TUpdateInputDto,
        // Event config
        TEntityCreateEvent,
        TEntityUpdateEvent,
        TWebhookEventEntity
        > : ApplicationService
        where TEntity : class, IEntity<Guid>, IIdentifiableEntity
        where TGetOutputDto : IEntityDto<Guid>
        where TListOutputDto : IEntityDto<Guid>
        where TEntityCreateEvent : AbstractEvent, new()
        where TEntityUpdateEvent : AbstractEvent, new()
        where TWebhookEventEntity : class, IWebhookQueueEntity, IEntity, new()
    {
        protected CrudApplicationService<
            TEntity,
            TGetOutputDto,
            TListOutputDto,
            TCreateInputDto,
            TUpdateInputDto,
            TEntityCreateEvent,
            TEntityUpdateEvent,
            TWebhookEventEntity
        > ParentAppService;

        protected AbstractApplicationService(
            IAbpLazyServiceProvider lazyServiceProvider,
            IEfCoreRepository<TEntity, Guid> repository,
            IObjectMapper objectMapper,
            AggregatedEventBus<TWebhookEventEntity> eventBus
        ) : base()
        {
            this.ParentAppService = new CrudApplicationService<
                TEntity,
                TGetOutputDto,
                TListOutputDto,
                TCreateInputDto,
                TUpdateInputDto,
                TEntityCreateEvent,
                TEntityUpdateEvent,
                TWebhookEventEntity
            >(lazyServiceProvider, repository, objectMapper, eventBus);
        }

        public async Task<SingleResponse<TGetOutputDto>> GetAsync(Guid id) {
            return new SingleResponse<TGetOutputDto>(await ParentAppService.GetAsync(id));
        }

        public async Task<CollectionResponse<TListOutputDto>> GetListAsync(Criteria input)
        {
            var response = await ParentAppService.MatchingAsync(input);
            return new CollectionResponse<TListOutputDto>()
            {
                Data = response,
                Pagination = new PaginationResponse()
                {
                    Total = response.TotalCount,
                    UnfilteredTotal = response.UnfilteredCount,
                    Page = input.Page != null ? input.Page.Page : 0,
                    Size = input.Page != null ? input.Page.Size : 0
                },
                Aggregations = response.Aggregations
            };
        }

        public Task<CollectionResponse<TListOutputDto>> PostSearchAsync(Criteria input)
        {
            return GetListAsync(input);
        }

        public async Task<SingleResponse<TGetOutputDto>> CreateAsync(TCreateInputDto input)
        {
            return new SingleResponse<TGetOutputDto>(await ParentAppService.CreateAsync(input));
        }

        public async Task<SingleResponse<TGetOutputDto>> UpdateAsync(Guid id, TUpdateInputDto input)
        {
            return new SingleResponse<TGetOutputDto>(await ParentAppService.UpdateAsync(id, input));
        }

        public async Task DeleteAsync(Guid id)
        {
            await ParentAppService.DeleteAsync(id);
        }
    }
}
