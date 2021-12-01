
using System.Threading.Tasks;
using RH.EntityFramework.Common;

namespace RH.EntityFramework.Repositories.Cycle
{
    public interface ICycleRepository
    {
        Task<PagedList<Shared.Entities.Cycle>> GetCyclesAsync(int page = 1, int pageSize = 10);
        Task<long> AddCycleAsync(Shared.Entities.Cycle cycle);
    }
}
