using System.Collections.Generic;
using System.Threading.Tasks;
using RH.EntityFramework.Shared.Entities;

namespace RH.EntityFramework.Repositories.Forecast.ECMWF
{
    public interface IEcmwfRepository
    {
        Task<List<Shared.Entities.Ecmwf>> GetForecastsByDimensionId(int dimensionId);
        Task<Shared.Entities.Ecmwf> Add(Shared.Entities.Ecmwf ecmwf);
        Task<WindyTime> GetTime(long start, short step);
        Task<WindyTime> GetLastExistTime(int dimensionId);
        Task<List<Ecmwf>> GetContentByDimensionAndTime(int dimensionId, int timeId);
    }
}
