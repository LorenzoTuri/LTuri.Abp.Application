using LTuri.Abp.Application.EntityFramework;

namespace LTuri.Abp.Application.Events.Webhook
{
    /// <summary>
    /// Event send before sending the event to weebhooks.
    /// By setting "SkipWebhooks" property to true, the event won't be sent.
    /// WARNING: NEVER implement the EventAttribute on this event, else a particolar
    ///     and difficult to discover circular reference will be created
    ///     (sending events will create new sendable events, that will fill the database in a n^2 speed)
    /// </summary>
    public class WebhookEventServiceWorkerBeforeSendEvent
    {
        public EventEntity Event { get; set; }
        public bool SkipWebhooks { get; set; } = false;
    }
}
