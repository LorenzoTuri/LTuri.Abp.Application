using LTuri.Abp.Application.EntityFramework.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Domain.Entities.Auditing;

namespace LTuri.Abp.Application.EntityFramework
{
    // TODO: check timestamps
    /// <summary>
    /// TODO: docs
    /// </summary>
    [Table("lturi_abp_application_event")]
    public class EventEntity : CreationAuditedAggregateRoot<Guid>, IMultiTenant
    {
        [Column("event_name")]
        [Required]
        public string EventName { get; set; } = "";

        [Column("event_status")]
        [Required]
        public EventStatus EventStatus { get; set; } = EventStatus.Pending;

        [Column("entity_name")]
        [Required]
        public string EntityName { get; set; } = "";

        [Column("entity_id")]
        [Required]
        public Guid? EntityId { get; set; }

        [Column("changed")]
        [Required]
        public string[] Changed { get; set; } = Array.Empty<string>();

        [Column("tenant_id")]
        [Required]
        public Guid? TenantId { get; set; }
    }
}
