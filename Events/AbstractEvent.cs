namespace LTuri.Abp.Application.Events
{
    [EventAttribute("", "")]
    public abstract class AbstractEvent
    {
        public Guid EntityId { get; set; }
        public string[] Changed { get; set; } = new string[0];
    }
}
