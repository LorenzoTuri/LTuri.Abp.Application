using LTuri.Abp.Application.Repositories.Criteria;
using LTuri.Abp.Application.Repositories.Criteria.Enum;

namespace LTuri.Abp.Application.Antlr.Query
{
    public class QueryPartFilter : IQueryPart
    {
        public QueryPartFilterType Type { get; set; }
        public string Property { get; set; } = "";
        public string Value { get; set; } = "";
        public CriteriaFilter ToCriteriaFilter()
        {
            var criteriaFilter = new CriteriaFilter()
            {
                Field = Property,
                Value = Value.Trim('\'').Trim('"')
            };
            switch (Type)
            {
                case QueryPartFilterType.Eq:
                case QueryPartFilterType.IEq:
                    criteriaFilter.Type = FilterType.Equals; break;
                case QueryPartFilterType.Contains:
                case QueryPartFilterType.IContains:
                    criteriaFilter.Type = FilterType.Contains; break;
                case QueryPartFilterType.Starts:
                case QueryPartFilterType.IStarts:
                    criteriaFilter.Type = FilterType.StartsWith; break;
                case QueryPartFilterType.Ends:
                case QueryPartFilterType.IEnds:
                    criteriaFilter.Type = FilterType.EndsWith; break;
                case QueryPartFilterType.Gt:
                case QueryPartFilterType.IGt:
                    criteriaFilter.Type = FilterType.Greater; break;
                case QueryPartFilterType.Lt:
                case QueryPartFilterType.ILt:
                    criteriaFilter.Type = FilterType.Lower; break;
                case QueryPartFilterType.Gte:
                case QueryPartFilterType.IGte:
                    criteriaFilter.Type = FilterType.GreaterEquals; break;
                case QueryPartFilterType.Lte:
                case QueryPartFilterType.ILte:
                    criteriaFilter.Type = FilterType.LowerEquals; break;
                case QueryPartFilterType.Full:
                case QueryPartFilterType.IFull:
                    criteriaFilter.Type = FilterType.FullText; break;
                case QueryPartFilterType.Any:
                case QueryPartFilterType.IAny:
                    criteriaFilter.Type = FilterType.Any; break;
            }
            switch (Type)
            {
                case QueryPartFilterType.Eq:
                case QueryPartFilterType.Contains:
                case QueryPartFilterType.Starts:
                case QueryPartFilterType.Ends:
                case QueryPartFilterType.Gt:
                case QueryPartFilterType.Lt:
                case QueryPartFilterType.Gte:
                case QueryPartFilterType.Lte:
                case QueryPartFilterType.Full:
                case QueryPartFilterType.Any:
                    criteriaFilter.Behaviour = FilterBehaviour.None; break;
                case QueryPartFilterType.IEq:
                case QueryPartFilterType.IContains:
                case QueryPartFilterType.IStarts:
                case QueryPartFilterType.IEnds:
                case QueryPartFilterType.IGt:
                case QueryPartFilterType.ILt:
                case QueryPartFilterType.IGte:
                case QueryPartFilterType.ILte:
                case QueryPartFilterType.IFull:
                case QueryPartFilterType.IAny:
                    criteriaFilter.Behaviour = FilterBehaviour.IgnoreCase; break;
            }
            return criteriaFilter;
        }
    }

    public enum QueryPartFilterType
    {
        Eq,
        IEq,
        
        Contains,
        IContains,
        
        Starts,
        IStarts,
        
        Ends,
        IEnds,
        
        Gt,
        IGt,
        
        Lt,
        ILt,
        
        Gte,
        IGte,
        
        Lte,
        ILte,
        
        Full,
        IFull,
        
        Any,
        IAny
    }
}
