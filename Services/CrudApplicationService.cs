using LTuri.Abp.Application.EntityTypes;
using LTuri.Abp.Application.Events;
using LTuri.Abp.Application.Events.EventBus;
using LTuri.Abp.Application.Repositories.Criteria;
using LTuri.Abp.Application.Repositories.Extensions;
using LTuri.Abp.Application.Repositories;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.ObjectMapping;

namespace LTuri.Abp.Application.Services
{
    /// <summary>
    /// Auto controller with:
    /// - Permission handling
    /// - Complex auto filters
    /// - Different DTOs for the different routes
    /// - Event hadling, by using custom entity provided with types
    /// - Support for multiple entities for events, if needed
    /// 
    /// TODO: check if can be written in a better way
    /// </summary>
    /// <typeparam name="TEntity">Base entity to build auto-api</typeparam>
    /// <typeparam name="TGetOutputDto">DTO for single result</typeparam>
    /// <typeparam name="TListOutputDto">DTO for multiple results</typeparam>
    /// <typeparam name="TCreateInputDto">Input DTO for entity creation</typeparam>
    /// <typeparam name="TUpdateInputDto">Input DTO for entity update</typeparam>
    /// <typeparam name="TEntityCreateEvent">Event triggered during creation</typeparam>
    /// <typeparam name="TEntityUpdateEvent">Event triggered during update</typeparam>
    public class CrudApplicationService<
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
       > : CrudApplicationService<
           TEntity,
           TGetOutputDto,
           TListOutputDto,
           TCreateInputDto,
           TUpdateInputDto
       >
       where TEntity : class, IEntity<Guid>, IIdentifiableEntity
       where TGetOutputDto : IEntityDto<Guid>
       where TListOutputDto : IEntityDto<Guid>
       where TEntityCreateEvent : AbstractEvent, new()
       where TEntityUpdateEvent : AbstractEvent, new()
    {
        protected AggregatedEventBus eventBus;

        public CrudApplicationService(
            IAbpLazyServiceProvider LazyServiceProvider,
            IEfCoreRepository<TEntity, Guid> repository,
            IObjectMapper objectMapper,
            AggregatedEventBus eventBus
        ) : base(LazyServiceProvider, repository, objectMapper)
        {
            this.eventBus = eventBus;
        }

        public override async Task<TGetOutputDto> UpdateAsync(Guid id, TUpdateInputDto input)
        {
            var result = await base.UpdateAsync(id, input);
            var changed = (await repository.GetChangelogAndSaveAsync()).ToArray();
            await eventBus.PublishAsync(new TEntityUpdateEvent()
            {
                EntityId = id,
                Changed = changed
            });
            return result;
        }

        public override async Task<TGetOutputDto> CreateAsync(TCreateInputDto input)
        {
            var result = await base.CreateAsync(input);
            await eventBus.PublishAsync(new TEntityCreateEvent()
            {
                EntityId = result.Id,
                Changed = Array.Empty<string>()
            });
            return result;
        }
    }

    /// <summary>
    /// Auto controller with:
    /// - Permission handling
    /// - Complex auto filters
    /// - Different DTOs for the different routes
    /// </summary>
    /// <typeparam name="TEntity">Base entity to build auto-api</typeparam>
    /// <typeparam name="TGetOutputDto">DTO for single result</typeparam>
    /// <typeparam name="TListOutputDto">DTO for multiple results</typeparam>
    /// <typeparam name="TCreateInputDto">Input DTO for entity creation</typeparam>
    /// <typeparam name="TUpdateInputDto">Input DTO for entity update</typeparam>
    public class CrudApplicationService<
       // Entity config
       TEntity,
       // Output config
       TGetOutputDto,
       TListOutputDto,
       // Input config
       TCreateInputDto,
       TUpdateInputDto
       > : CrudAppService<
           TEntity,
           TGetOutputDto,
           TListOutputDto,
           Guid,
           Criteria,
           TCreateInputDto,
           TUpdateInputDto
       >
       where TEntity : class, IEntity<Guid>, IIdentifiableEntity
       where TGetOutputDto : IEntityDto<Guid>
       where TListOutputDto : IEntityDto<Guid>
    {
        protected IEfCoreRepository<TEntity, Guid> repository;
        protected IObjectMapper objectMapper;

        public CrudApplicationService(
            IAbpLazyServiceProvider LazyServiceProvider,
            IEfCoreRepository<TEntity, Guid> repository,
            IObjectMapper objectMapper
        ) : base(repository)
        {
            this.LazyServiceProvider = LazyServiceProvider;
            this.repository = repository;
            this.objectMapper = objectMapper;
        }

        public override async Task<TGetOutputDto> GetAsync(Guid id)
        {
            await CheckGetPolicyAsync();
            var entity = await GetEntityByIdAsync(id);
            var result = await MapToGetOutputDtoAsync(entity);
            return result;
        }

        public override async Task<TGetOutputDto> UpdateAsync(Guid id, TUpdateInputDto input)
        {
            await CheckUpdatePolicyAsync();
            var entity = await GetEntityByIdAsync(id);
            await MapToEntityAsync(input, entity);
            await repository.UpdateAsync(entity);
            return await MapToGetOutputDtoAsync(entity);
        }

        public override async Task<TGetOutputDto> CreateAsync(TCreateInputDto input)
        {
            var result = await base.CreateAsync(input);
            return result;
        }

        public async Task<TGetOutputDto> GetByIdentifierAsync(string identifier)
        {
            // TODO: find a better way, this way the full list is loaded into RAM before returning
            var entity = (await repository.ToListAsync()).FirstOrDefault(x => x.Identifier == identifier);
            if (entity == null) throw new EntityNotFoundException(typeof(TEntity), identifier);
            return await base.GetAsync(entity.Id);
        }

        public override async Task<PagedResultDto<TListOutputDto>> GetListAsync(Criteria input)
        {
            var result = await MatchingAsync(input);
            return new PagedResultDto<TListOutputDto>() {
                Items = result,
                TotalCount = result.TotalCount
            };
        }

        public Task<PagedResultDto<TListOutputDto>> PostSearchAsync(Criteria input)
        {
            return GetListAsync(input);
        }

        public async Task<MatchingResult<TListOutputDto>> MatchingAsync(Criteria input)
        {
            var entities = await repository.MatchingAsync(input);
            return new MatchingResult<TListOutputDto>(
                objectMapper.Map<IEnumerable<TEntity>, IEnumerable<TListOutputDto>>(entities)
            ) {
                TotalCount = entities.TotalCount,
                UnfilteredCount = entities.UnfilteredCount,
                Aggregations = entities.Aggregations
            };
        }

        public string GetPolicy(PolicyType policy)
        {
            return policy switch
            {
                PolicyType.Get => GetPolicyName,
                PolicyType.List => GetListPolicyName,
                PolicyType.Create => CreatePolicyName,
                PolicyType.Update => UpdatePolicyName,
                PolicyType.Delete => DeletePolicyName,
                _ => throw new ArgumentOutOfRangeException(nameof(policy)),
            };
        }

        public void SetPolicy(PolicyType policy, string value)
        {
            switch (policy)
            {
                case PolicyType.Get: GetPolicyName = value; break;
                case PolicyType.List: GetListPolicyName = value; break;
                case PolicyType.Create: CreatePolicyName = value; break;
                case PolicyType.Update: UpdatePolicyName = value; break;
                case PolicyType.Delete: DeletePolicyName = value; break;
                default: throw new ArgumentOutOfRangeException(nameof(policy));
            }
        }
    }

    public enum PolicyType
    {
        Get,
        List,
        Create,
        Update,
        Delete
    }
}
