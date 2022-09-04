using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace LTuri.Abp.Application.EntityFramework
{
    [ConnectionStringName("LTuriAbpApplication")]
    public class LTuriAbpApplicationDbContext : 
        AbpDbContext<LTuriAbpApplicationDbContext>, 
        ILTuriAbpApplicationDbContext
    {
        public DbSet<EventEntity> Events { get; set; }

        public DbSet<WebhookEntity> Webhooks { get; set; }

        public LTuriAbpApplicationDbContext(DbContextOptions<LTuriAbpApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ConfigureLTuriAbpApplication();
        }
    }

}
