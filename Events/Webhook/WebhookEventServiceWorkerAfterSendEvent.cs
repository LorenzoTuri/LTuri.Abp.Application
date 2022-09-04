using LTuri.Abp.Application.EntityFramework;

namespace LTuri.Abp.Application.Events.Webhook
{
    /// <summary>
    /// Event sent after sending the event to weebhooks.
    /// WARNING: NEVER implement the EventAttribute on this event, else a particolar
    ///     and difficult to discover circular reference will be created
    ///     (sending events will create new sendable events, that will fill the database in a n^2 speed)
    /// </summary>
    public class WebhookEventServiceAfterBeforeSendEvent
    {
        public EventEntity Event { get; set; }
    }
}
