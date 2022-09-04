namespace LTuri.Abp.Application.Events
{
    /// <summary>
    /// TODO: should by interface instead of abstract? maybe
    /// </summary>
    [EventAttribute("", "")]
    public abstract class AbstractEvent
    {
        public Guid EntityId { get; set; }
        public string[] Changed { get; set; } = new string[0];
    }
}
