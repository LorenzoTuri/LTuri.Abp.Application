using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;

namespace LTuri.Abp.Application.Repositories.Extensions
{
    public static class IEfCoreRepository
    {
        public static async Task<IEnumerable<string>> GetChangelogAndSaveAsync<TEntity, TKey>(
            this IEfCoreRepository<TEntity, TKey> repository
        ) where TEntity : class, IEntity<TKey>
        {
            var context = await repository.GetDbContextAsync();
            List<string> changeLog = new List<string>();
            var changedEntities = context.ChangeTracker.Entries().Where(e =>
                e.State == EntityState.Modified
            ).ToList();
            changedEntities.ForEach(change => {
                foreach (var propertyName in change.OriginalValues.Properties.Select(property => property.Name))
                {
                    var original = change.OriginalValues[propertyName];
                    var current = change.CurrentValues[propertyName];

                    if (original != null && current != null && original.GetType().IsAssignableTo<Dictionary<string, object>>())
                    {
                        if (!((Dictionary<string, object>)original).SequenceEqual((Dictionary<string, object>)current))
                            changeLog.Add(propertyName);
                    }
                    else
                    {
                        if (!Equals(original, current))
                        {
                            changeLog.Add(propertyName);
                        }
                    }
                }
            });
            await context.SaveChangesAsync();

            return changeLog;
        }
    }
}
