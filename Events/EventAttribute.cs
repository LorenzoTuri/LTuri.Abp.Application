namespace LTuri.Abp.Application.Events
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EventAttribute : Attribute
    {
        public string EventName { get; set; }
        public string EntityName { get; set; }

        public EventAttribute(
            string EventName,
            string EntityName
        )
        {
            this.EventName = EventName;
            this.EntityName = EntityName;
        }
    }
}
