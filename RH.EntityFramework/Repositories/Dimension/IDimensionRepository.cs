using System.Collections.Generic;
using System.Threading.Tasks;
using RH.EntityFramework.Common;

namespace RH.EntityFramework.Repositories.Dimension
{
    public interface IDimensionRepository
    {
        Task<PagedList<Shared.Entities.Dimension>> GetDimensionsAsync(int page = 1, int pageSize = 10);
        List<Shared.Entities.Dimension> GetAllActiveDimensions();

        Task<Shared.Entities.Dimension> this[int id] { get; }

        Task<int> AddDimensionAsync(Shared.Entities.Dimension dimension);

        Task<bool> ActivateDimensionAsync(int dimensionId);

        Task<bool> DeactivateDimensionAsync(int dimensionId);

        Task<EntityFramework.Shared.Entities.Dimension> GetDimension(short zoom, short x, short y);
    }
}
