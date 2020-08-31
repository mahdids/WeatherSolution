using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RH.EntityFramework.Shared.DbContexts;
using RH.EntityFramework.Shared.Entities;

namespace RH.EntityFramework.Repositories.Forecast.GFS
{
    public class GfsRepository:IGfsRepository
    {
        private WeatherDbContext _dbContext;

        public GfsRepository(WeatherDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<Gfs>> GetForecastsByDimensionId(int dimensionId)
        {
            return _dbContext.Gfses.Where(x => x.DimensionId == dimensionId).ToListAsync();
        }

        public async Task<Gfs> Add(Gfs gfs)
        {
            var dbRecord =await _dbContext.Gfses.FirstOrDefaultAsync(x => x.DimensionId == gfs.DimensionId
                                                                     && x.Location == gfs.Location
                                                                     && x.WindyTimeId == gfs.WindyTimeId);
            if (dbRecord==null)
            {
                _dbContext.Gfses.Add(gfs);
            }
            else
            {
                dbRecord.DataString = gfs.DataString;
                dbRecord.RegisterDate = gfs.RegisterDate;
                gfs.Id = dbRecord.Id;
            }

            await _dbContext.SaveChangesAsync();
            return gfs;
        }

        public async Task<WindyTime> GetTime(long start, short step)
        {
            var dbRecord = await _dbContext.WindyTimes.FirstOrDefaultAsync(x => x.Start == start && x.Step == step && x.Type == "GFS");
            if (dbRecord == null)
            {
                dbRecord = new WindyTime()
                {
                    Start = start,
                    Step = step,
                    Type = "GFS"
                };
                _dbContext.WindyTimes.Add(dbRecord);
                await _dbContext.SaveChangesAsync();
            }

            return dbRecord;
        }

        public async Task<WindyTime> GetLastExistTime(int dimensionId)
        {
            if (!_dbContext.Gfses.Where(x => x.DimensionId == dimensionId).Any())
                return null;
            var maxTimeId = _dbContext.Gfses.Where(x => x.DimensionId == dimensionId).Max(x => x.WindyTimeId);
            return await _dbContext.WindyTimes.FirstOrDefaultAsync(x => x.Id == maxTimeId);
        }

        public async Task<List<Gfs>> GetContentByDimensionAndTime(int dimensionId, int timeId)
        {
            return await _dbContext.Gfses.Where(x => x.DimensionId == dimensionId && x.WindyTimeId == timeId)
                .ToListAsync();
        }
    }
}
