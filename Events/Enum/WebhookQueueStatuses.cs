using System;
using System.Collections.Generic;
using System.Text;

namespace LTuri.Abp.Application.Events.Enum
{
    public enum WebhookQueueStatuses
    {
        Pending,
        Running,
        Success,
        Error
    }
}
