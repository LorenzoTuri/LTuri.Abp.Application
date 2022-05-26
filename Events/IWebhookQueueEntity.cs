using LTuri.Abp.Application.Events.Enum;

namespace LTuri.Abp.Application.Events
{
    public interface IWebhookQueueEntity
    {
        public string EventName { get; set; }
        public WebhookQueueStatuses EventStatus { get; set; }
        public string EntityName { get; set; }
        public Guid? EntityId { get; set; }
        public string[] Changed { get; set; }
        public Guid? TenantId { get; set; }
    }
}
