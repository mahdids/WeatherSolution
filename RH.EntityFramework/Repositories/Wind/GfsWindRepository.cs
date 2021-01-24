using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RH.EntityFramework.Shared.DbContexts;
using RH.EntityFramework.Shared.Entities;

namespace RH.EntityFramework.Repositories.Wind
{
    public class GfsWindRepository
    {
        private WeatherDbContext _dbContext;

        public GfsWindRepository(WeatherDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<GfsForecast>> GetForecastsByDimensionId(int dimensionId)
        {
            return _dbContext.GfsForecasts.Where(x => x.WindDimensionId == dimensionId).ToListAsync();
        }

        public async Task<GfsForecast> Add(GfsForecast gfs)
        {
            var dbRecord = await _dbContext.GfsForecasts.FirstOrDefaultAsync(x => x.WindDimensionId== gfs.WindDimensionId
                                                                      && x.DateTime == gfs.DateTime&&x.ReferenceTime==gfs.ReferenceTime);
            if (dbRecord == null)
            {
                _dbContext.GfsForecasts.Add(gfs);
            }
            else
            {
                gfs.Id = dbRecord.Id;
            }

            await _dbContext.SaveChangesAsync();
            return gfs;
        }

        //public async Task<WindyTime> GetTime(long start, short step)
        //{
        //    var dbRecord = await _dbContext.WindyTimes.FirstOrDefaultAsync(x => x.Start == start && x.Step == step && x.Type == "GFS");
        //    if (dbRecord == null)
        //    {
        //        dbRecord = new WindyTime()
        //        {
        //            Start = start,
        //            Step = step,
        //            Type = "GFS"
        //        };
        //        _dbContext.WindyTimes.Add(dbRecord);
        //        await _dbContext.SaveChangesAsync();
        //    }

        //    return dbRecord;
        //}

        //public async Task<WindyTime> GetLastExistTime(int dimensionId)
        //{
        //    if (!_dbContext.Gfses.Any(x => x.DimensionId == dimensionId))
        //        return null;
        //    var maxTimeId = _dbContext.Gfses.Where(x => x.DimensionId == dimensionId).Max(x => x.WindyTimeId);
        //    var time = await _dbContext.WindyTimes.FirstOrDefaultAsync(x => x.Id == maxTimeId);

        //    return time;
        //}

        //public async Task<List<WindyTime>> GetExistTime(int dimensionId, long prevDay, long nextDay)
        //{
        //    return await _dbContext.WindyTimes.Where(x => x.Start >= prevDay && x.Start <= nextDay && x.Type == "GFS")
        //        .ToListAsync();
        //}

        public async Task<List<GfsForecast>> GetContentByDimensionAndTime(int dimensionId, DateTime time)
        {
            return await _dbContext.GfsForecasts.Where(x => x.WindDimensionId == dimensionId && x.ReferenceTime== time)
                .ToListAsync();
        }
        public async Task<List<GfsForecast>> GetContentByDimensionAndEpoc(int dimensionId, long time)
        {
            return await _dbContext.GfsForecasts.Where(x => x.WindDimensionId == dimensionId && x.OrigTs== time).OrderBy(x=>x.ReferenceTime)
                .ToListAsync();
        }

        public async Task<DateTime> GetLastExistTime(int dimensionId)
        {
            return await _dbContext.GfsForecasts.Where(x => x.WindDimensionId == dimensionId).MaxAsync(x => x.ReferenceTime);
        }

        public async Task<long> GetNearestTime(int dimensionId, long epocTime)
        {
            var minDiff =await _dbContext.GfsForecasts.Where(x => x.WindDimensionId == dimensionId)
                .MinAsync(x => Math.Abs(x.OrigTs - epocTime));
            var record=await _dbContext.GfsForecasts.FirstOrDefaultAsync(x => Math.Abs(x.OrigTs - epocTime) == minDiff);
            return record.OrigTs;

        }
    }
}
