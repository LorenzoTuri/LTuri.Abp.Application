using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace LTuri.Abp.Application.EntityFramework
{
    /// <summary>
    /// TODO: docs
    /// TODO: timestamps
    /// </summary>
    [Table("lturi_abp_application_webhook")]
    public class WebhookEntity : AggregateRoot<Guid>, IMultiTenant
    {   
        #region Related entity data
        /// <summary>
        /// Identifier used in case there are correlated entities
        /// (Ex. multiple external apps/plugins with webhook)
        /// </summary>
        [Column("related_entity")]
        public string? RelatedEntity { get; set; }

        /// <summary>
        /// As RelatedEntity, but containing the id of the correlated entity
        /// </summary>
        [Column("related_entity_id")]
        public Guid? RelatedEntityId { get; set; }
        #endregion

        #region Webhook specific data
        [Column("event_name")]
        [Required]
        [MinLength(1), MaxLength(50)]
        public string? EventName { get; set; }

        [Column("is_active")]
        [Required]
        public bool IsActive { get; set; } = true;
        
        [Column("method")]
        [Required]
        public Enum.HttpMethod Method { get; set; } = Enum.HttpMethod.Get;
        
        [Column("url")]
        [Required]
        [MinLength(1), MaxLength(5000)]
        public string? Url { get; set; }
        #endregion

        #region MultiTenancy
        [Column("tenant_id")]
        [Required]
        public Guid? TenantId { get; set; }
        #endregion
    }
}
