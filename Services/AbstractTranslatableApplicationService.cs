using LTuri.Abp.Application.EntityTypes;
using LTuri.Abp.Application.Events;
using LTuri.Abp.Application.Events.EventBus;
using LTuri.Abp.Application.Services.Response;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.ObjectMapping;

namespace LTuri.Abp.Application.Services
{
    /// <summary>
    /// TODO: translations routes are still missing
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TEntityTranslation"></typeparam>
    /// <typeparam name="TGetOutputDto"></typeparam>
    /// <typeparam name="TListOutputDto"></typeparam>
    /// <typeparam name="TEntityTranslationOutputDto"></typeparam>
    /// <typeparam name="TCreateInputDto"></typeparam>
    /// <typeparam name="TUpdateInputDto"></typeparam>
    /// <typeparam name="TEntityCreateEvent"></typeparam>
    /// <typeparam name="TEntityUpdateEvent"></typeparam>
    /// <typeparam name="TWebHookEventEntity"></typeparam>
    public abstract class AbstractTranslatableApplicationService<
        // Entity config
        TEntity,
        // Entity translations config
        TEntityTranslation,
        // Output config
        TGetOutputDto,
        TListOutputDto,
        TEntityTranslationOutputDto,
        // Input config
        TCreateInputDto,
        TUpdateInputDto,
        // Event config
        TEntityCreateEvent,
        TEntityUpdateEvent,
        TWebHookEventEntity
        > : AbstractApplicationService<
            TEntity,
            TGetOutputDto,
            TListOutputDto,
            TCreateInputDto,
            TUpdateInputDto,
            TEntityCreateEvent,
            TEntityUpdateEvent,
            TWebHookEventEntity
            >
        where TEntity : class, IEntity<Guid>, ITranslatableEntity<TEntityTranslation>, IIdentifiableEntity
        where TEntityTranslation : ITranslationEntity
        where TGetOutputDto : IEntityDto<Guid>
        where TListOutputDto : IEntityDto<Guid>
        where TEntityCreateEvent : AbstractEvent, new()
        where TEntityUpdateEvent : AbstractEvent, new()
        where TWebHookEventEntity : class, IWebhookQueueEntity, IEntity, new()
    {

        protected IEfCoreRepository<TEntity, Guid> repository;
        protected IObjectMapper objectMapper;

        protected AbstractTranslatableApplicationService(
            IAbpLazyServiceProvider lazyServiceProvider,
            IEfCoreRepository<TEntity, Guid> repository,
            IObjectMapper objectMapper,
            AggregatedEventBus<TWebHookEventEntity> eventBus
        ) : base(lazyServiceProvider, repository, objectMapper, eventBus)
        {
            this.repository = repository;
            this.objectMapper = objectMapper;
        }

        public async Task<CollectionResponse<TEntityTranslationOutputDto>> GetTranslations(Guid id)
        {
            // Check for permissions
            await AuthorizationService.CheckAsync(ParentAppService.GetPolicy(PolicyType.Get));

            // Then return entity translations
            var entity = await repository.GetAsync(id);
            var data = objectMapper.Map<
                IEnumerable<TEntityTranslation>,
                IEnumerable<TEntityTranslationOutputDto>
            >(entity.Translations);

            return new CollectionResponse<TEntityTranslationOutputDto>()
            {
                Data = data,
                Pagination = new PaginationResponse()
                {
                    Total = entity.Translations.Count,
                    Page = 0,
                    Size = entity.Translations.Count
                }
            };
        }
    }

    public abstract class AbstractTranslatableApplicationService<
        // Entity config
        TEntity,
        // Entity translations config
        TEntityTranslation,
        // Output config
        TGetOutputDto,
        TListOutputDto,
        TEntityTranslationOutputDto,
        // Input config
        TCreateInputDto,
        TUpdateInputDto
        > : AbstractApplicationService<
            TEntity,
            TGetOutputDto,
            TListOutputDto,
            TCreateInputDto,
            TUpdateInputDto
            >
        where TEntity : class, IEntity<Guid>, ITranslatableEntity<TEntityTranslation>, IIdentifiableEntity
        where TEntityTranslation : ITranslationEntity
        where TGetOutputDto : IEntityDto<Guid>
        where TListOutputDto : IEntityDto<Guid>
    {

        protected IEfCoreRepository<TEntity, Guid> repository;
        protected IObjectMapper objectMapper;

        protected AbstractTranslatableApplicationService(
            IAbpLazyServiceProvider lazyServiceProvider,
            IEfCoreRepository<TEntity, Guid> repository,
            IObjectMapper objectMapper
        ) : base(lazyServiceProvider, repository, objectMapper)
        {
            this.repository = repository;
            this.objectMapper = objectMapper;
        }

        public async Task<CollectionResponse<TEntityTranslationOutputDto>> GetTranslations(Guid id)
        {
            // Check for permissions
            await AuthorizationService.CheckAsync(ParentAppService.GetPolicy(PolicyType.Get));

            // Then return entity translations
            var entity = await repository.GetAsync(id);
            var data = objectMapper.Map<
                IEnumerable<TEntityTranslation>,
                IEnumerable<TEntityTranslationOutputDto>
            >(entity.Translations);

            return new CollectionResponse<TEntityTranslationOutputDto>()
            {
                Data = data,
                Pagination = new PaginationResponse()
                {
                    Total = entity.Translations.Count,
                    Page = 0,
                    Size = entity.Translations.Count
                }
            };
        }
    }
}
