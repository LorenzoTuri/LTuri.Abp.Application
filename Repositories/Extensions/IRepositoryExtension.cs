using Antlr4.Runtime;
using LTuri.Abp.Application.Antlr.Query;
using LTuri.Abp.Application.Repositories;
using LTuri.Abp.Application.Repositories.Criteria.Enum;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace LTuri.Abp.Application.Repositories.Extensions
{
    public static class IRepositoryExtension
    {
        public static async Task<MatchingResult<TEntity>> MatchingAsync<TEntity, TKey>(
            this IRepository<TEntity, TKey> repository,
            Criteria.Criteria criteria
        ) where TEntity : class, IEntity<TKey>
        {
            IQueryable<TEntity> queryable = await repository.GetQueryableAsync();
            Dictionary<string, object> aggregations = new();

            // Apply sortings
            if (criteria.Sortings != null)
            {
                queryable = queryable.Order(criteria.Sortings);
            }

            // Need to use the filters server side, not database side...
            // When ToString will be supported by EntityFramework, we can return to IQueryable and remove .Compile()
            IEnumerable<TEntity> enumerable = queryable.AsEnumerable();

            // Apply pre aggregations
            if (criteria.Aggregations != null)
            {
                var applier = new AggregationApplier<TEntity, TKey>();
                foreach (var aggregation in criteria.Aggregations.Where(x => x.Behaviour == AggregationBehaviour.Pre))
                    aggregations[aggregation.Name] = applier.Apply(queryable, aggregation);
            }

            // Apply filters
            if (criteria.Filters != null)
            {
                var applier = new FilterApplier<TEntity>();
                foreach (var filter in criteria.Filters)
                    enumerable = applier.Apply(enumerable, filter);
            }

            // Let's apply the query
            if (criteria.Query.Trim().Length > 0)
            {
                var visitor = new QueryVisitor();
                var queryCriteria = visitor.BuildCriteria(criteria.Query);

                if (queryCriteria.Filters != null)
                {
                    var applier = new FilterApplier<TEntity>();
                    foreach (var filter in queryCriteria.Filters)
                        enumerable = applier.Apply(enumerable, filter);
                }
            }

            // Apply post aggregations
            if (criteria.Aggregations != null)
            {
                var applier = new AggregationApplier<TEntity, TKey>();
                foreach (var aggregation in criteria.Aggregations.Where(x => x.Behaviour == AggregationBehaviour.Post))
                    aggregations[aggregation.Name] = applier.Apply(enumerable, aggregation);
            }

            long filteredCount = enumerable.Count();
            long unfilteredCount = queryable.Count();

            // Apply pagination
            if (criteria.Page != null)
            {
                enumerable = enumerable.
                    Skip((criteria.Page.Page - 1) * criteria.Page.Size).
                    Take(criteria.Page.Size);
            }

            return new MatchingResult<TEntity>(enumerable)
            {
                TotalCount = filteredCount,
                UnfilteredCount = unfilteredCount,
                Aggregations = aggregations
            };
        }
    }
}
