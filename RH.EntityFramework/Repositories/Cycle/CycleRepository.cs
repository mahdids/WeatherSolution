using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RH.EntityFramework.Common;
using RH.EntityFramework.Shared.DbContexts;

namespace RH.EntityFramework.Repositories.Cycle
{
    public class CycleRepository : ICycleRepository
    {
        private WeatherDbContext _dbContext;
        private ILogger<CycleRepository> _logger;
        public CycleRepository(WeatherDbContext dbContext, ILogger<CycleRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public async Task<long> AddCycleAsync(Shared.Entities.Cycle cycle)
        {
            if (cycle.Id == 0)
            {
                var newCycle = new Shared.Entities.Cycle()
                {
                    Type = cycle.Type,
                    StartTime = cycle.StartTime,
                    dateTime = cycle.dateTime,
                    Compeleted = cycle.Compeleted
                };
                _dbContext.Cycles.Add(newCycle);
                await _dbContext.SaveChangesAsync();
                cycle.Id = newCycle.Id;
                return cycle.Id;
            }
            var currentCycle = await _dbContext.Cycles.FirstOrDefaultAsync(x => x.Id == cycle.Id);
            if (currentCycle == null)
                return 0;
            currentCycle.dateTime = cycle.dateTime;
            currentCycle.EndTime = cycle.EndTime;
            currentCycle.Compeleted = cycle.Compeleted;
            await _dbContext.SaveChangesAsync();
            return currentCycle.Id;
        }

        public async Task<PagedList<Shared.Entities.Cycle>> GetCyclesAsync(int page = 1, int pageSize = 10)
        {
            return await PagedList<Shared.Entities.Cycle>.ToPagedList(_dbContext.Cycles.OrderByDescending(x=>x.StartTime), page, pageSize);
        }
    }
}
