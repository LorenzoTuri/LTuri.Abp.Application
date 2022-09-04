namespace LTuri.Abp.Application.EntityFramework.Enum
{
    // TODO: Success & Error are still needed? 
    // Probably can be removed, or a more complex scenario with decoupling of event
    // and eventWebhook request must be made. In any case, success is always not possible,
    // since on success the row get's deleted
    public enum EventStatus
    {
        Pending,
        Running,
        Success,
        Error
    }
}
