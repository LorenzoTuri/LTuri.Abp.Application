using LTuri.Abp.Application.Repositories.Criteria.Enum;

namespace LTuri.Abp.Application.Repositories.Criteria
{
    /// <summary>
    /// Rappresent a single filter
    /// Depending on Type, the properties Field, Value and Filters are used
    /// TODO: Field lowecase, only after invariant case is implemented
    /// </summary>
    public class CriteriaFilter
    {
        public FilterType Type { get; set; } = FilterType.Equals;
        public FilterBehaviour Behaviour { get; set; } = FilterBehaviour.None;
        public string Field { get; set; } = "Id";
        public string Value { get; set; } = "";
        public IEnumerable<CriteriaFilter> Filters { get; set; } = new List<CriteriaFilter>();
    }
}
