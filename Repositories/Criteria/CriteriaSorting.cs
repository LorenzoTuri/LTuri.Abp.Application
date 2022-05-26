using LTuri.Abp.Application.Repositories.Criteria.Enum;

namespace LTuri.Abp.Application.Repositories.Criteria
{
    public class CriteriaSorting
    {
        public string By { get; set; } = "Id";
        public SortingDirection Direction { get; set; } = SortingDirection.Desc;
    }
}
