using LTuri.Abp.Application.EntityFramework;
using LTuri.Abp.Application.ServiceWorker;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Modularity;

namespace LTuri.Abp.Application;

public class LTuriAbpApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<LTuriAbpApplicationDbContext>(options =>
        {
            // TODO: required??? also is Webhook missing or what? ^^"
            options.AddRepository<EventEntity, EventEntityRepository>();
        });
    }

    public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        await context.AddBackgroundWorkerAsync<WebhookEventServiceWorker>();
    }
}
