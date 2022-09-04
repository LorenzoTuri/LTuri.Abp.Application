using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace LTuri.Abp.Application.EntityFramework
{
    [ConnectionStringName("LTuriAbpApplication")]
    public interface ILTuriAbpApplicationDbContext : IEfCoreDbContext
    {
        DbSet<EventEntity> Events { get; }
        DbSet<WebhookEntity> Webhooks { get; }
    }
}
