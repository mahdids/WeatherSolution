using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RH.EntityFramework.Common;

namespace RH.EntityFramework.Repositories.Dimension
{
    public interface IWindDimensionRepository
    {
        Task<PagedList<Shared.Entities.WindDimension>> GetDimensionsAsync(int page = 1, int pageSize = 10);
        List<Shared.Entities.WindDimension> GetAllActiveDimensions();

        Task<Shared.Entities.WindDimension> this[int id] { get; }

        Task<int> AddDimensionAsync(Shared.Entities.WindDimension dimension);

        Task<bool> ActivateDimensionAsync(int dimensionId);

        Task<bool> DeactivateDimensionAsync(int dimensionId);

        Task<EntityFramework.Shared.Entities.WindDimension> GetDimension(double x,double y, bool autoAdd = true);
    }
}
