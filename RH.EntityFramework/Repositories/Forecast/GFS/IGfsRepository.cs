using System.Collections.Generic;
using System.Threading.Tasks;
using RH.EntityFramework.Shared.Entities;

namespace RH.EntityFramework.Repositories.Forecast.GFS
{
    public interface IGfsRepository
    {
        Task<List<Shared.Entities.Gfs>> GetForecastsByDimensionId(int dimensionId);
        Task<Shared.Entities.Gfs> Add(Shared.Entities.Gfs gfs);
        Task<WindyTime> GetTime(long start, short step);

        Task<WindyTime> GetLastExistTime(int dimensionId);
        Task<List<WindyTime>> GetExistTime(int dimensionId,long prevDay,long nextDay);
        Task<List<Gfs>> GetContentByDimensionAndTime(int dimensionId, int timeId);
    }
}
