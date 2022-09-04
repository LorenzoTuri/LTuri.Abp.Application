using LTuri.Abp.Application.Repositories.Criteria.Enum;

namespace LTuri.Abp.Application.Repositories.Criteria
{
    /// <summary>
    /// TODO: By lowercase, do only when invariant case is implemented
    /// </summary>
    public class CriteriaSorting
    {
        public string By { get; set; } = "Id";
        public SortingDirection Direction { get; set; } = SortingDirection.Desc;
    }
}
