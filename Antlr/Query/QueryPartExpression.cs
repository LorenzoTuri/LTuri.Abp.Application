using LTuri.Abp.Application.Repositories.Criteria;
using LTuri.Abp.Application.Repositories.Criteria.Enum;

namespace LTuri.Abp.Application.Antlr.Query
{
    public class QueryPartExpression : IQueryPart
    {
        public QueryPartExpressionType Type { get; set; }
        public QueryPartFilter? Filter { get; set; }
        public IEnumerable<QueryPartExpression> Expressions { get; set; } = new List<QueryPartExpression>();

        public CriteriaFilter ToCriteriaFilter()
        {
            if (Type == QueryPartExpressionType.Filter && Filter != null)
            {
                return Filter.ToCriteriaFilter();
            }
            if (Type == QueryPartExpressionType.Not)
            {
                return new CriteriaFilter()
                {
                    Type = FilterType.Not,
                    Filters = Expressions.Select(x => x.ToCriteriaFilter())
                };
            }

            var criteriaFilter = new CriteriaFilter()
            {
                Type = (Type == QueryPartExpressionType.Or) ? FilterType.Or : FilterType.And,
                Filters = Expressions.Select(x => x.ToCriteriaFilter())
            };
            return criteriaFilter;
        }
    }

    public enum QueryPartExpressionType
    {
        Filter,
        Not,
        Parentheses,
        And,
        Or
    }
}
