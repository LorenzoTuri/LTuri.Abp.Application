using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace LTuri.Abp.Application.EntityFramework
{
    /// <summary>
    /// TODO: is this needed?
    /// </summary>
    public class WebhookEntityRepository
    : EfCoreRepository<ILTuriAbpApplicationDbContext, WebhookEntity, Guid>, IWebhookEntityRepository
    {
        public WebhookEntityRepository(IDbContextProvider<ILTuriAbpApplicationDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }

}
