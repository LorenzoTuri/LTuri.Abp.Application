using LTuri.Abp.Application.Repositories.Criteria;
using LTuri.Abp.Application.Exceptions;
using LTuri.Abp.Application.Repositories.Criteria.Enum;
using System.Linq.Expressions;
using System.Reflection;

namespace LTuri.Abp.Application.Repositories.Extensions
{
    public static class IQueryable
    {
        public static IQueryable<T> Order<T>(this IQueryable<T> source, IEnumerable<CriteriaSorting> sortings)
        {
            var type = typeof(T);
            foreach (var item in sortings.Select((value, i) => (value, i)))
            {
                source = Order(
                    source,
                    type,
                    item.value,
                    item.i == 0
                );
            }
            return source;
        }

        /// <summary>
        /// TODO: 0 references? can be removed?
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="sorting"></param>
        /// <returns></returns>
        public static IQueryable<T> Order<T>(this IQueryable<T> source, CriteriaSorting sorting)
        {
            var type = typeof(T);

            return Order(
                source,
                type,
                sorting,
                true
            );
        }

        /// <summary>
        /// TODO: find invariant case?
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="type"></param>
        /// <param name="sorting"></param>
        /// <param name="firstSortingCall"></param>
        /// <returns></returns>
        /// <exception cref="SortingByWrongParameterException"></exception>
        private static IQueryable<T> Order<T>(
            IQueryable<T> source,
            Type type,
            CriteriaSorting sorting,
            bool firstSortingCall
        )
        {
            var property = type.GetProperty(sorting.By, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            var parameter = Expression.Parameter(type, "p");

            if (property == null) throw new SortingByWrongParameterException(sorting.By, type.Name);

            try
            {
                // Detect method used for sorting
                string method = sorting.Direction == SortingDirection.Asc ? "OrderBy" : "OrderByDescending";
                if (!firstSortingCall) method = sorting.Direction == SortingDirection.Asc ? "ThenBy" : "ThenByDescending";

                // Then sort the enumerable
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                MethodCallExpression resultExp = Expression.Call(
                    typeof(Queryable),
                    method,
                    new Type[] { type, property.PropertyType },
                    source.Expression,
                    Expression.Quote(orderByExp)
                );
                return source.Provider.CreateQuery<T>(resultExp);
            }
            catch (ArgumentNullException ex)
            {
                throw new SortingByWrongParameterException(sorting.By, type.Name, ex);
            }
        }
    }
}
