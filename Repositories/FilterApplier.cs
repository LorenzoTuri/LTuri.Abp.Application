using LTuri.Abp.Application.Exceptions;
using LTuri.Abp.Application.Repositories.Criteria;
using LTuri.Abp.Application.Repositories.Criteria.Enum;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using Volo.Abp;

namespace LTuri.Abp.Application.Repositories
{
    public class FilterApplier<TEntity>
    {
        protected ParameterExpression ExpressionParameter { get; set; }

        public FilterApplier()
        {
            // TODO: is it possible to query on subentities?
            ExpressionParameter = Expression.Parameter(typeof(TEntity), "obj");
        }


        public IEnumerable<TEntity> Apply(
            IEnumerable<TEntity> queryable, 
            CriteriaFilter filter
        ) 
        {
            queryable = queryable.AsEnumerable();
            // Encapsulate this exception in an Userfriendly exception to allow client to see the actual error
            // This exception relates to type conversion only and are safe to be printed out
            try
            {
                return queryable.Where(DetectSimpleFilter(queryable, filter).Compile());
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(
                    ex.Message,
                    "400",
                    null,
                    ex
                );
            }
        }

        /// <summary>
        /// TODO: invariant case?
        /// </summary>
        /// <param name="queryable"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private Expression<Func<TEntity, bool>> DetectSimpleFilter(
            IEnumerable<TEntity> queryable,
            CriteriaFilter filter
        )
        {
            var objProperty = Expression.PropertyOrField(ExpressionParameter, filter.Field);

            return filter.Type switch
            {
                // Simple filters
                FilterType.Equals => Equal(objProperty, filter.Value, filter.Behaviour),
                FilterType.Contains => Contains(objProperty, filter.Value, filter.Behaviour),
                FilterType.StartsWith => StartsWith(objProperty, filter.Value, filter.Behaviour),
                FilterType.EndsWith => EndsWith(objProperty, filter.Value, filter.Behaviour),
                FilterType.Greater => Greater(objProperty, filter.Value, filter.Behaviour),
                FilterType.Lower => Lower(objProperty, filter.Value, filter.Behaviour),
                FilterType.GreaterEquals => GreaterEquals(objProperty, filter.Value, filter.Behaviour),
                FilterType.LowerEquals => LowerEquals(objProperty, filter.Value, filter.Behaviour),
                // Groups
                FilterType.Any => Or(filter.Value.ToString().Split(",").Select(
                    x => Equal(objProperty, x, filter.Behaviour)
                ).ToArray()),
                FilterType.FullText => Or(typeof(TEntity).GetProperties().Select(
                    x => Contains(objProperty, filter.Value, filter.Behaviour)
                ).ToArray()),// TODO: exclude properties by annotation?
                // Recursion
                FilterType.Not => Not(And(filter.Filters.Select(
                    x => DetectSimpleFilter(queryable, x)
                ).ToArray())),
                FilterType.And => And(filter.Filters.Select(
                    x => DetectSimpleFilter(queryable, x)
                ).ToArray()),
                FilterType.Or => Or(filter.Filters.Select(
                    x => DetectSimpleFilter(queryable, x)
                ).ToArray()),
                // Should really not be here
                _ => throw new NotImplementedException($"Filter type {filter.Type} not implemented now"),
            };
        }

        /// <summary>
        /// Extract a lambda that uses a EQUAL filter
        /// </summary>
        private Expression<Func<TEntity, bool>> Equal(
            MemberExpression property,
            object value,
            FilterBehaviour behaviour
        )
        {
            var valueExpression = FilterApplier<TEntity>.ComplexCastExpression(property, value, behaviour);
            var expression = (behaviour.HasFlag(FilterBehaviour.IgnoreCase)) ?
                Expression.Equal(FilterApplier<TEntity>.CallToStringMethod(property, behaviour), valueExpression) :
                Expression.Equal(property, valueExpression);
            return Expression.Lambda<Func<TEntity, bool>>(expression, ExpressionParameter);
        }

        /// <summary>
        /// Extract a lambda that uses a CONTAINS filter
        /// </summary>
        private Expression<Func<TEntity, bool>> Contains(
            MemberExpression property,
            object value,
            FilterBehaviour behaviour
        )
        {
            return StringFilter(
                property,
                value,
                behaviour,
                "Contains"
            );
        }

        /// <summary>
        /// Extract a lambda that uses a CONTAINS filter
        /// </summary>
        private Expression<Func<TEntity, bool>> StartsWith(
            MemberExpression property,
            object value,
            FilterBehaviour behaviour
        )
        {
            return StringFilter(
                property,
                value,
                behaviour,
                "StartsWith"
            );
        }

        /// <summary>
        /// Extract a lambda that uses a CONTAINS filter
        /// </summary>
        private Expression<Func<TEntity, bool>> EndsWith(
            MemberExpression property,
            object value,
            FilterBehaviour behaviour
        )
        {
            return StringFilter(
                property,
                value,
                behaviour,
                "EndsWith"
            );
        }

        /// <summary>
        /// Extract a lambda that uses a GREATER filter
        /// </summary>
        private Expression<Func<TEntity, bool>> Greater(
            MemberExpression property,
            object value,
            FilterBehaviour behaviour
        )
        {
            var valueExpression = FilterApplier<TEntity>.ComplexCastExpression(property, value, behaviour);
            var expression = (behaviour.HasFlag(FilterBehaviour.IgnoreCase)) ?
                Expression.GreaterThan(FilterApplier<TEntity>.CallToStringMethod(property, behaviour), valueExpression) :
                Expression.GreaterThan(property, valueExpression);
            return Expression.Lambda<Func<TEntity, bool>>(expression, ExpressionParameter);
        }

        /// <summary>
        /// Extract a lambda that uses a LOWER filter
        /// </summary>
        private Expression<Func<TEntity, bool>> Lower(
            MemberExpression property,
            object value,
            FilterBehaviour behaviour
        )
        {
            var valueExpression = FilterApplier<TEntity>.ComplexCastExpression(property, value, behaviour);
            var expression = (behaviour.HasFlag(FilterBehaviour.IgnoreCase)) ?
                Expression.LessThan(FilterApplier<TEntity>.CallToStringMethod(property, behaviour), valueExpression) :
                Expression.LessThan(property, valueExpression);
            return Expression.Lambda<Func<TEntity, bool>>(expression, ExpressionParameter);
        }

        /// <summary>
        /// Extract a lambda that uses a GREATER EQUALS filter
        /// </summary>
        private Expression<Func<TEntity, bool>> GreaterEquals(
            MemberExpression property,
            object value,
            FilterBehaviour behaviour
        )
        {
            var valueExpression = FilterApplier<TEntity>.ComplexCastExpression(property, value, behaviour);
            var expression = (behaviour.HasFlag(FilterBehaviour.IgnoreCase)) ?
                Expression.GreaterThanOrEqual(FilterApplier<TEntity>.CallToStringMethod(property, behaviour), valueExpression) :
                Expression.GreaterThanOrEqual(property, valueExpression);
            return Expression.Lambda<Func<TEntity, bool>>(expression, ExpressionParameter);
        }

        /// <summary>
        /// Extract a lambda that uses a LOWER EQUALS filter
        /// </summary>
        private Expression<Func<TEntity, bool>> LowerEquals(
            MemberExpression property,
            object value,
            FilterBehaviour behaviour
        )
        {
            var valueExpression = FilterApplier<TEntity>.ComplexCastExpression(property, value, behaviour);
            var expression = (behaviour.HasFlag(FilterBehaviour.IgnoreCase)) ?
                Expression.LessThanOrEqual(FilterApplier<TEntity>.CallToStringMethod(property, behaviour), valueExpression) :
                Expression.LessThanOrEqual(property, valueExpression);
            return Expression.Lambda<Func<TEntity, bool>>(expression, ExpressionParameter);
        }

        private Expression<Func<TEntity, bool>> Not(
            Expression<Func<TEntity, bool>> basicExpression
        )
        {
            var negateExpression = Expression.Not(basicExpression.Body);
            return Expression.Lambda<Func<TEntity, bool>>(negateExpression, ExpressionParameter);
        }

        /// <summary>
        /// Extract a lambda that uses AND concatenations
        /// </summary>
        private Expression<Func<TEntity, bool>> And(
            Expression<Func<TEntity, bool>>[] predicates
        )
        {
            Expression body = Expression.Invoke(predicates[0], ExpressionParameter);
            for (int i = 1; i < predicates.Length; i++)
            {
                body = Expression.AndAlso(body, Expression.Invoke(predicates[i], ExpressionParameter));
            }
            return Expression.Lambda<Func<TEntity, bool>>(body, ExpressionParameter);
        }

        /// <summary>
        /// Extract a lambda that uses OR concatenations
        /// </summary>
        private Expression<Func<TEntity, bool>> Or(
            Expression<Func<TEntity, bool>>[] predicates
        )
        {
            Expression body = Expression.Invoke(predicates[0], ExpressionParameter);
            for (int i = 1; i < predicates.Length; i++)
            {
                body = Expression.OrElse(body, Expression.Invoke(predicates[i], ExpressionParameter));
            }
            return Expression.Lambda<Func<TEntity, bool>>(body, ExpressionParameter);
        }

        private Expression<Func<TEntity, bool>> StringFilter(
            MemberExpression property,
            object value,
            FilterBehaviour behaviour,
            string functionName
        )
        {
            var stringExpr = FilterApplier<TEntity>.CallToStringMethod(property, behaviour);
            if (behaviour.HasFlag(FilterBehaviour.IgnoreCase)) value = ((string)value).ToString().ToLower();

            var stringMethod = typeof(string).GetMethod(functionName, new[] { typeof(string) });

            if (stringMethod == null) throw new CallToMissingMethodException(functionName, property.Member.Name);

            var methodCall = Expression.Call(
                stringExpr,
                stringMethod,
                Expression.Constant(value, typeof(string))
            );
            return Expression.Lambda<Func<TEntity, bool>>(methodCall, ExpressionParameter);
        }

        private static MethodCallExpression CallToStringMethod(MemberExpression property, FilterBehaviour behaviour)
        {
            var toStringMethod = property.Type.GetMethod("ToString", Array.Empty<Type>());
            if (toStringMethod == null) throw new CallToMissingMethodException("ToString", property.Member.Name);
            if (behaviour.HasFlag(FilterBehaviour.IgnoreCase))
            {
                var toLowerMethod = typeof(string).GetMethod("ToLower", Array.Empty<Type>());
                if (toLowerMethod == null) throw new CallToMissingMethodException("ToLower", property.Member.Name);
                return Expression.Call(Expression.Call(property, toStringMethod), toLowerMethod);
            }
            return Expression.Call(property, toStringMethod);
        }

        private static Expression ComplexCastExpression(
            MemberExpression property,
            object value,
            FilterBehaviour behaviour
        )
        {
            if (behaviour.HasFlag(FilterBehaviour.IgnoreCase))
            {
                var propertyType = ((PropertyInfo)property.Member).PropertyType;
                var converter = TypeDescriptor.GetConverter(propertyType);
                if (!converter.CanConvertFrom(typeof(string)))
                    throw new FilterValueTypeConversionException(
                        value?.ToString()!,
                        propertyType.Name
                    );
                var propertyValue = converter.ConvertFrom(value.ToString()!.ToLower());
                var constant = Expression.Constant(propertyValue);
                return Expression.Convert(constant, propertyType);
            }
            else
            {
                var propertyType = ((PropertyInfo)property.Member).PropertyType;
                var converter = TypeDescriptor.GetConverter(propertyType);
                if (!converter.CanConvertFrom(typeof(string)))
                    throw new FilterValueTypeConversionException(
                        value.ToString()!,
                        propertyType.Name
                    );
                var propertyValue = converter.ConvertFrom(value);
                var constant = Expression.Constant(propertyValue);
                return Expression.Convert(constant, propertyType);
            }
        }
    }
}
