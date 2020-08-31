using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RH.EntityFramework.Shared.DbContexts;
using RH.EntityFramework.Shared.Entities;

namespace RH.EntityFramework.Repositories.Forecast.ECMWF
{
    public class EcmwfRepository:IEcmwfRepository
    {
        private readonly WeatherDbContext _dbContext;

        public EcmwfRepository(WeatherDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<Ecmwf>> GetForecastsByDimensionId(int dimensionId)
        {
            return _dbContext.Ecmwfs.Where(x => x.DimensionId == dimensionId).ToListAsync();
        }

        public async Task<Ecmwf> Add(Ecmwf ecmwf)
        {
            var dbRecord = await _dbContext.Ecmwfs.FirstOrDefaultAsync(x => x.DimensionId == ecmwf.DimensionId
                                                                           && x.Location == ecmwf.Location
                                                                           && x.WindyTimeId == ecmwf.WindyTimeId);
            if (dbRecord == null)
            {
                _dbContext.Ecmwfs.Add(ecmwf);
            }
            else
            {
                dbRecord.DataString = ecmwf.DataString;
                dbRecord.RegisterDate = ecmwf.RegisterDate;
                ecmwf.Id = dbRecord.Id;
            }

            await _dbContext.SaveChangesAsync();
            return ecmwf;
        }

        public async Task<WindyTime> GetTime(long start, short step)
        {
            var dbRecord= await _dbContext.WindyTimes.FirstOrDefaultAsync(x => x.Start == start && x.Step == step && x.Type == "ECMWF");
            if (dbRecord==null)
            {
                dbRecord=new WindyTime()
                {
                    Start = start,
                    Step=step,
                    Type = "ECMWF"
                };
                _dbContext.WindyTimes.Add(dbRecord);
                await _dbContext.SaveChangesAsync();
            }

            return dbRecord;
        }

        public async Task<WindyTime> GetLastExistTime(int dimensionId)
        {
            if (!_dbContext.Ecmwfs.Where(x => x.DimensionId == dimensionId).Any())
                return null;
            var maxTimeId = _dbContext.Ecmwfs.Where(x => x.DimensionId == dimensionId).Max(x => x.WindyTimeId);
            return await _dbContext.WindyTimes.FirstOrDefaultAsync(x => x.Id == maxTimeId);
        }

        public async Task<List<Ecmwf>> GetContentByDimensionAndTime(int dimensionId, int timeId)
        {
            return await _dbContext.Ecmwfs.Where(x => x.DimensionId == dimensionId && x.WindyTimeId == timeId)
                .ToListAsync();
        }
    }

}
