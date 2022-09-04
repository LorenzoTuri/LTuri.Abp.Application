using LTuri.Abp.Application.Exceptions;
using LTuri.Abp.Application.Repositories.Criteria;
using LTuri.Abp.Application.Repositories.Criteria.Enum;
using System.Linq.Expressions;
using Volo.Abp.Domain.Entities;

namespace LTuri.Abp.Application.Repositories
{
    public class AggregationApplier<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
    {
        public object Apply(IEnumerable<TEntity> queryable, CriteriaAggregation aggregation)
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TEntity));
            var propertyInfo = typeof(TEntity).GetProperty(aggregation.Field);

            if (propertyInfo == null) throw new FilterByWrongParameterException(aggregation.Field, typeof(TEntity).Name);

            var expression = Expression.Property(parameterExpression, propertyInfo);

            // TODO: fully test everything (maybe with different types)
            // TODO: also return must be better formatted...
            // TODO: doesn't comply to SOLID
            return aggregation.Type switch
            {
                AggregationType.Avg => AggregationApplier<TEntity, TKey>.Avg(queryable, parameterExpression, expression),
                AggregationType.Count => AggregationApplier<TEntity, TKey>.Count(queryable, parameterExpression, expression),
                AggregationType.Max => AggregationApplier<TEntity, TKey>.Max(queryable, parameterExpression, expression),
                AggregationType.Min => AggregationApplier<TEntity, TKey>.Min(queryable, parameterExpression, expression),
                AggregationType.Sum => AggregationApplier<TEntity, TKey>.Sum(queryable, parameterExpression, expression),
                AggregationType.Stats => AggregationApplier<TEntity, TKey>.Stats(queryable, parameterExpression, expression),
                AggregationType.Group => AggregationApplier<TEntity, TKey>.Group(queryable, parameterExpression, expression),
                AggregationType.Histogram => AggregationApplier<TEntity, TKey>.Histogram(queryable, parameterExpression, expression),
                _ => throw new NotImplementedException(),
            };
        }

        /// <summary>
        /// Average of all numeric values for the specified field
        /// </summary>
        private static object Avg(
            IEnumerable<TEntity> queryable,
            ParameterExpression parameterExpression,
            MemberExpression property
        )
        {
            var avg = queryable.Average(AggregationApplier<TEntity, TKey>.PropertyLambda<double>(parameterExpression, property));
            return new
            {
                Avg = avg
            };
        }
        /// <summary>
        /// Number of records for the specified field (where is not null)
        /// </summary>
        private static object Count(
            IEnumerable<TEntity> queryable,
            ParameterExpression parameterExpression,
            MemberExpression property
        )
        {
            var count = queryable.Count(AggregationApplier<TEntity, TKey>.PropertyNotNullLambda(parameterExpression, property));
            return new
            {
                Count = count
            };
        }
        /// <summary>
        /// Maximum value for the specified field
        /// </summary>
        private static object Max(
            IEnumerable<TEntity> queryable,
            ParameterExpression parameterExpression,
            MemberExpression property
        )
        {
            var max = queryable.Max(AggregationApplier<TEntity, TKey>.PropertyLambda<double>(parameterExpression, property));
            return new
            {
                Max = max
            };
        }
        /// <summary>
        /// Minimal value for the specified field
        /// </summary>
        private static object Min(
            IEnumerable<TEntity> queryable,
            ParameterExpression parameterExpression,
            MemberExpression property
        )
        {
            var min = queryable.Min(AggregationApplier<TEntity, TKey>.PropertyLambda<double>(parameterExpression, property));
            return new
            {
                Min = min
            };
        }
        /// <summary>
        /// Sum of all numeric values for the specified field
        /// </summary>
        private static object Sum(
            IEnumerable<TEntity> queryable,
            ParameterExpression parameterExpression,
            MemberExpression property
        )
        {
            var sum = queryable.Sum(AggregationApplier<TEntity, TKey>.PropertyLambda<double>(parameterExpression, property));
            return new
            {
                Sum = sum
            };
        }
        /// <summary>
        /// Stats overall numeric values for the specified field (Avg, Count, Max, Min, Sum)
        /// </summary>
        private static object Stats(
            IEnumerable<TEntity> queryable,
            ParameterExpression parameterExpression,
            MemberExpression property
        )
        {
            var propertyLambda = AggregationApplier<TEntity, TKey>.PropertyLambda<double>(parameterExpression, property);
            var propertyNotNullLambda = AggregationApplier<TEntity, TKey>.PropertyNotNullLambda(parameterExpression, property);
            var avg = queryable.Average(propertyLambda);
            var count = queryable.Count(propertyNotNullLambda);
            var max = queryable.Max(propertyLambda);
            var min = queryable.Min(propertyLambda);
            var sum = queryable.Sum(propertyLambda);
            return new
            {
                Avg = avg,
                Count = count,
                Max = max,
                Min = min,
                Sum = sum
            };
        }
        /// <summary>
        /// Groups the results for each value of the provided field
        /// </summary>
        private static object Group(
            IEnumerable<TEntity> queryable,
            ParameterExpression parameterExpression,
            MemberExpression property
        )
        {
            var group = queryable.GroupBy(AggregationApplier<TEntity, TKey>.PropertyLambda<string>(parameterExpression, property));

            return group.ToHashSet().Select(x =>
                new {
                    x.Key,
                    Values = x.Select(y => y.Id)
                }
            );
        }
        /// <summary>
        /// Groups the result for each value of the provided field, by using an histogram aggregation
        /// (grouped by minute, hour, day, week, month, quarter, year, day)
        /// </summary>
        private static object Histogram(
            IEnumerable<TEntity> queryable,
            ParameterExpression parameterExpression,
            MemberExpression property
        )
        {
            string toStringFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss";

            var toStringMethod = property.Type.GetMethod("ToString", new Type[] { typeof(string) });

            if (toStringMethod == null) 
                throw new FilterByWrongParameterException(property.Member.Name+".ToString", typeof(TEntity).Name);

            var toStringExpression = Expression.Call(
                property,
                toStringMethod,
                Expression.Constant(toStringFormat)
            );

            var toStringLambda = Expression.Lambda<Func<TEntity, bool>>(toStringExpression, parameterExpression).Compile();
            return queryable.GroupBy(toStringLambda);
        }

        private static Func<TEntity, TType> PropertyLambda<TType>(
            ParameterExpression parameterExpression, 
            MemberExpression property
        )
        {
            if (typeof(TType).Equals(typeof(string))) {
                var toStringMethod = property.Type.GetMethod("ToString", Array.Empty<Type>());
                
                if (toStringMethod == null)
                    throw new FilterByWrongParameterException(property.Member.Name + ".ToString", typeof(TEntity).Name);

                var toStringExpression = Expression.Call(property, toStringMethod);
                return Expression.Lambda<Func<TEntity, TType>>(toStringExpression, parameterExpression).Compile();
            }
            else
            {
                var castExpression = Expression.Convert(property, typeof(TType));
                return Expression.Lambda<Func<TEntity, TType>>(castExpression, parameterExpression).Compile();
            }
        }
        private static Func<TEntity, bool> PropertyNotNullLambda(
            ParameterExpression parameterExpression,
            MemberExpression property
        )
        {
            var castExpression = Expression.Convert(property, typeof(object));
            var notNullExpression = Expression.NotEqual(castExpression, Expression.Constant(null));
            return Expression.Lambda<Func<TEntity, bool>>(notNullExpression, parameterExpression).Compile();
        }
    }
}
