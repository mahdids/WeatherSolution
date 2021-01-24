using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RH.EntityFramework.Common;
using RH.EntityFramework.Shared.DbContexts;
using RH.EntityFramework.Shared.Entities;

namespace RH.EntityFramework.Repositories.Dimension
{
    public class WindDimensionRepository : IWindDimensionRepository
    {
        private readonly WeatherDbContext _dbContext;
        private ILogger<WindDimensionRepository> _logger;

        public WindDimensionRepository(WeatherDbContext dbContext, ILogger<WindDimensionRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<PagedList<WindDimension>> GetDimensionsAsync(int page = 1, int pageSize = 10)
        {
            return await PagedList<Shared.Entities.WindDimension>.ToPagedList(_dbContext.WindDimensions, page, pageSize);
        }

        public List<WindDimension> GetAllActiveDimensions() => _dbContext.WindDimensions.Where(x => x.IsActive).ToList();

        public Task<WindDimension> this[int id] => throw new NotImplementedException();

        public async Task<int> AddDimensionAsync(WindDimension dimension)
        {
            var dbRecord = await _dbContext.WindDimensions.FirstOrDefaultAsync(x =>
                x.X == dimension.X && x.Y == dimension.Y);
            if (dbRecord == null)
            {
                await _dbContext.WindDimensions.AddAsync(dimension);
                await _dbContext.SaveChangesAsync();
                return dimension.Id;
            }

            dbRecord.IsActive = true;
            await _dbContext.SaveChangesAsync();
            dimension.Id = dbRecord.Id;
            return dimension.Id;
        }

        public async Task<bool> ActivateDimensionAsync(int dimensionId)
        {
            var dimension = await _dbContext.WindDimensions.FirstOrDefaultAsync(x => x.Id == dimensionId);
            if (dimension != null)
            {
                dimension.IsActive = true;
                await _dbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> DeactivateDimensionAsync(int dimensionId)
        {
            var dimension = await _dbContext.WindDimensions.FirstOrDefaultAsync(x => x.Id == dimensionId);
            if (dimension != null)
            {
                dimension.IsActive = false;
                await _dbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<WindDimension> GetDimension(double x, double y, bool autoAdd = true)
        {
            var dimension =
                await _dbContext.WindDimensions.FirstOrDefaultAsync(d => Math.Abs(d.X - x) <= 0.5 && Math.Abs(d.Y - y) <= 0.5);
            if (dimension == null && autoAdd)
            {
                dimension = new Shared.Entities.WindDimension()
                {
                    X = x,
                    Y = y,
                    IsActive = false
                };
                _dbContext.WindDimensions.Add(dimension);
                await _dbContext.SaveChangesAsync();
            }
            return dimension;
        }
    }
}
