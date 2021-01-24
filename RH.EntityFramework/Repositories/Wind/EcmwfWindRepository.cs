using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RH.EntityFramework.Shared.DbContexts;
using RH.EntityFramework.Shared.Entities;

namespace RH.EntityFramework.Repositories.Wind
{
    public class EcmwfWindRepository
    {
        private WeatherDbContext _dbContext;

        public EcmwfWindRepository(WeatherDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<EcmwfForecast>> GetForecastsByDimensionId(int dimensionId)
        {
            return _dbContext.EcmwfForecasts.Where(x => x.WindDimensionId == dimensionId).ToListAsync();
        }

        public async Task<EcmwfForecast> Add(EcmwfForecast ecmwf)
        {
            var dbRecord = await _dbContext.EcmwfForecasts.FirstOrDefaultAsync(x => x.WindDimensionId == ecmwf.WindDimensionId
                                                                      && x.DateTime == ecmwf.DateTime && x.ReferenceTime == ecmwf.ReferenceTime);
            if (dbRecord == null)
            {
                _dbContext.EcmwfForecasts.Add(ecmwf);
            }
            else
            {
                ecmwf.Id = dbRecord.Id;
            }

            await _dbContext.SaveChangesAsync();
            return ecmwf;
        }

       

        public async Task<List<EcmwfForecast>> GetContentByDimensionAndTime(int dimensionId, DateTime time)
        {
            return await _dbContext.EcmwfForecasts.Where(x => x.WindDimensionId == dimensionId && x.ReferenceTime == time)
                .ToListAsync();
        }

        public async Task<List<EcmwfForecast>> GetContentByDimensionAndEpoc(int dimensionId, long time)
        {
            return await _dbContext.EcmwfForecasts.Where(x => x.WindDimensionId == dimensionId && x.OrigTs == time).OrderBy(x => x.ReferenceTime)
                .ToListAsync();
        }

        public async Task<DateTime> GetLastExistTime(int dimensionId)
        {
            return await _dbContext.EcmwfForecasts.Where(x => x.WindDimensionId == dimensionId).MaxAsync(x => x.ReferenceTime);
        }

        public async Task<long> GetNearestTime(int dimensionId, long epocTime)
        {
            var minDiff = await _dbContext.EcmwfForecasts.Where(x => x.WindDimensionId == dimensionId)
                .MinAsync(x => Math.Abs(x.OrigTs - epocTime));
            var record = await _dbContext.EcmwfForecasts.FirstOrDefaultAsync(x => Math.Abs(x.OrigTs - epocTime) == minDiff);
            return record.OrigTs;

        }
    }
}
