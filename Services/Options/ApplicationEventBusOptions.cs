using LTuri.Abp.Application.Events.EventBus;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Local;

namespace LTuri.Abp.Application.Services.Options
{
    /// <summary>
    /// TODO: is it the right way to do it?
    /// </summary>
    public class ApplicationEventBusOptions
    {
        public List<IEventBus> GetEventBuses(
            IServiceProvider services
        )
        {
            var list = new List<IEventBus>();

            var localEventBus = services.GetService<ILocalEventBus>();
            if (localEventBus != null) list.Add(localEventBus);

            var webhookEventBus = services.GetService<WebhookEventBus>();
            if (webhookEventBus != null) list.Add(webhookEventBus);
            
            return list;
        }
    }
}
