using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Diagnostics.CodeAnalysis;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace LTuri.Abp.Application.EntityFramework
{
    /// <summary>
    /// TODO: docs
    /// </summary>
    public static class LTuriAbpApplicationDbContextModelBuilderExtensions
    {
        public static void ConfigureLTuriAbpApplication([NotNull] this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.Entity<EventEntity>(b =>
            {
                b.ConfigureByConvention();

                b.Property(e => e.Changed).HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                );
            });

            builder.Entity<WebhookEntity>()
                .Property(d => d.Method)
                .HasConversion(new EnumToStringConverter<Enum.HttpMethod>());
        }
    }
}
