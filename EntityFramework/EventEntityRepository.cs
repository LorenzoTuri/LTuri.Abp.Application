using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace LTuri.Abp.Application.EntityFramework
{
    /// <summary>
    /// TODO: is this needed?
    /// </summary>
    public class EventEntityRepository
    : EfCoreRepository<ILTuriAbpApplicationDbContext, EventEntity, Guid>, IEventEntityRepository
    {
        public EventEntityRepository(IDbContextProvider<ILTuriAbpApplicationDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }

}
