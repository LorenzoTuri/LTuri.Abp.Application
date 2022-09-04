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
    /// <summary>
    /// Build an auto api with auto filters and event handler
    /// Full permissions are also respected
    /// 
    /// TODO: check if can be written in a different way (maybe differentiate into 2 different projects?)
    /// TODO: do I maybe need rotues like count/ids?
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TGetOutputDto"></typeparam>
    /// <typeparam name="TListOutputDto"></typeparam>
    /// <typeparam name="TCreateInputDto"></typeparam>
    /// <typeparam name="TUpdateInputDto"></typeparam>
    /// <typeparam name="TEntityCreateEvent"></typeparam>
    /// <typeparam name="TEntityUpdateEvent"></typeparam>
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
        TEntityUpdateEvent
        > : ApplicationService
        where TEntity : class, IEntity<Guid>, IIdentifiableEntity
        where TGetOutputDto : IEntityDto<Guid>
        where TListOutputDto : IEntityDto<Guid>
        where TEntityCreateEvent : AbstractEvent, new()
        where TEntityUpdateEvent : AbstractEvent, new()
    {
        protected CrudApplicationService<
            TEntity,
            TGetOutputDto,
            TListOutputDto,
            TCreateInputDto,
            TUpdateInputDto,
            TEntityCreateEvent,
            TEntityUpdateEvent
        > ParentAppService;

        protected AbstractApplicationService(
            IAbpLazyServiceProvider lazyServiceProvider,
            IEfCoreRepository<TEntity, Guid> repository,
            IObjectMapper objectMapper,
            AggregatedEventBus eventBus
        ) : base()
        {
            this.ParentAppService = new CrudApplicationService<
                TEntity,
                TGetOutputDto,
                TListOutputDto,
                TCreateInputDto,
                TUpdateInputDto,
                TEntityCreateEvent,
                TEntityUpdateEvent
            >(lazyServiceProvider, repository, objectMapper, eventBus);
        }

        public async Task<SingleResponse<TGetOutputDto>> GetAsync(Guid id) {
            return new SingleResponse<TGetOutputDto>(await ParentAppService.GetAsync(id));
        }

        public async Task<SingleResponse<TGetOutputDto>> GetByIdentifierAsync(string identifier)
        {
            return new SingleResponse<TGetOutputDto>(await ParentAppService.GetByIdentifierAsync(identifier));
        }

        public Task<CollectionResponse<TListOutputDto>> GetListAsync(Criteria input)
        {
            return PostSearchAsync(input);
        }

        public async Task<CollectionResponse<TListOutputDto>> PostSearchAsync(Criteria input)
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

    /// <summary>
    /// Build an auto api with auto filters and event handler
    /// Full permissions are also respected
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TGetOutputDto"></typeparam>
    /// <typeparam name="TListOutputDto"></typeparam>
    /// <typeparam name="TCreateInputDto"></typeparam>
    /// <typeparam name="TUpdateInputDto"></typeparam>
    public abstract class AbstractApplicationService<
        // Entity config
        TEntity,
        // Output config
        TGetOutputDto,
        TListOutputDto,
        // Input config
        TCreateInputDto,
        TUpdateInputDto
        > : ApplicationService
        where TEntity : class, IEntity<Guid>, IIdentifiableEntity
        where TGetOutputDto : IEntityDto<Guid>
        where TListOutputDto : IEntityDto<Guid>
    {
        protected CrudApplicationService<
            TEntity,
            TGetOutputDto,
            TListOutputDto,
            TCreateInputDto,
            TUpdateInputDto
        > ParentAppService;

        protected AbstractApplicationService(
            IAbpLazyServiceProvider lazyServiceProvider,
            IEfCoreRepository<TEntity, Guid> repository,
            IObjectMapper objectMapper
        ) : base()
        {
            this.ParentAppService = new CrudApplicationService<
                TEntity,
                TGetOutputDto,
                TListOutputDto,
                TCreateInputDto,
                TUpdateInputDto
            >(lazyServiceProvider, repository, objectMapper);
        }

        public async Task<SingleResponse<TGetOutputDto>> GetAsync(Guid id)
        {
            return new SingleResponse<TGetOutputDto>(await ParentAppService.GetAsync(id));
        }

        public async Task<SingleResponse<TGetOutputDto>> GetByIdentifierAsync(string identifier)
        {
            return new SingleResponse<TGetOutputDto>(await ParentAppService.GetByIdentifierAsync(identifier));
        }

        public Task<CollectionResponse<TListOutputDto>> GetListAsync(Criteria input)
        {
            return PostSearchAsync(input);
        }

        public async Task<CollectionResponse<TListOutputDto>> PostSearchAsync(Criteria input)
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
