using LTuri.Abp.Application.Repositories.Criteria;

namespace LTuri.Abp.Application.Repositories.Criteria
{
    /// <summary>
    /// Rappresents filters on the IQueriable
    /// 
    /// TODO: return type? (xml,json, csv etc...) --> tried but not working, not prio
    /// </summary>
    public class Criteria
    {
        public IEnumerable<CriteriaSorting>? Sortings { get; set; } = new List<CriteriaSorting>();
        public CriteriaPagination? Page { get; set; } = new CriteriaPagination();
        public IEnumerable<CriteriaFilter>? Filters { get; set; } = new List<CriteriaFilter>();
        public IEnumerable<CriteriaAggregation>? Aggregations { get; set; } = new List<CriteriaAggregation>();
        public string Query { get; set; } = string.Empty;
    }
}
