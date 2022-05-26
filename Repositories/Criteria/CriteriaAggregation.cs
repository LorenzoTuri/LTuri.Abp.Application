using LTuri.Abp.Application.Repositories.Criteria.Enum;
using System.ComponentModel.DataAnnotations;

namespace LTuri.Abp.Application.Repositories.Criteria
{
    /// <summary>
    /// Request for an aggregation over the data.
    /// </summary>
    public class CriteriaAggregation
    {
        [Required]
        public AggregationType Type { get; set; } = AggregationType.Avg;
        public AggregationBehaviour Behaviour { get; set; } = AggregationBehaviour.Post;
        [Required]
        public string Name { get; set; } = "";
        [Required]
        public string Field { get; set; } = "";
    }
}
