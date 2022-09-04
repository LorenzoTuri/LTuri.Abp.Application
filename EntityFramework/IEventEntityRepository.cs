using Volo.Abp.Domain.Repositories;

namespace LTuri.Abp.Application.EntityFramework
{
    public interface IEventEntityRepository : IRepository<EventEntity, Guid>
    {
    }
}
