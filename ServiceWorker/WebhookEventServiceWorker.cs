using LTuri.Abp.Application.Services.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;
using Volo.Abp.Threading;

namespace LTuri.Abp.Application.ServiceWorker
{
    public class WebhookEventServiceWorker : AsyncPeriodicBackgroundWorkerBase
    {
        private static Semaphore semaphore = new Semaphore(0, 1);

        public WebhookEventServiceWorker(
            AbpAsyncTimer timer,
            IServiceScopeFactory serviceScopeFactory
        ) : base(
            timer,
            serviceScopeFactory
        )
        {
            Timer.Period = 60 * 1000; // each 1 minute
        }

        protected async override Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            Logger.LogInformation("WebhookEventServiceWorker: Sending webhook events...");

            var HasLock = semaphore.WaitOne(TimeSpan.FromSeconds(5));
            if (HasLock)
            {
                var sender = workerContext.ServiceProvider.GetRequiredService<WebhookSenderService>();
                var tenantRepository = workerContext.ServiceProvider.GetRequiredService<IRepository<Tenant, Guid>>();
                var currentTenant = workerContext.ServiceProvider.GetRequiredService<ICurrentTenant>();

                var tenants = (await tenantRepository.GetQueryableAsync()).ToList();

                foreach (var tenant in tenants)
                {
                    using (currentTenant.Change(tenant.Id))
                    {
                        await sender.Send();
                    }
                }

                semaphore.Release();
                Logger.LogInformation("WebhookEventServiceWorker: Completed...");
            }
            else
            {
                Logger.LogInformation("WebhookEventServiceWorker: Semaphore is still locked, skipping...");
            }
        }
    }
}
