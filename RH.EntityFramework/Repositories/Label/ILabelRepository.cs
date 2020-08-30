using System.Collections.Generic;
using System.Threading.Tasks;

namespace RH.EntityFramework.Repositories.Label
{
    public interface ILabelRepository
    {
        Task<List<Shared.Entities.Label>> GetLabelsByDimensionId(int dimensionId);
        Task<Shared.Entities.Label> Add(Shared.Entities.Label label);
    }
}
