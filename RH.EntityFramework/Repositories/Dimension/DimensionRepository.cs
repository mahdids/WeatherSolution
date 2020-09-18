using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RH.EntityFramework.Common;
using RH.EntityFramework.Shared.DbContexts;

namespace RH.EntityFramework.Repositories.Dimension
{
    public class DimensionRepository : IDimensionRepository
    {
        private WeatherDbContext _dbContext;
        private ILogger<DimensionRepository> _logger;

        public DimensionRepository(WeatherDbContext dbContext, ILogger<DimensionRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<PagedList<Shared.Entities.Dimension>> GetDimensionsAsync(int page = 1, int pageSize = 10)
        {
            return await PagedList<Shared.Entities.Dimension>.ToPagedList(_dbContext.Dimensions, page, pageSize);
        }

        public List<Shared.Entities.Dimension> GetAllActiveDimensions()
            => _dbContext.Dimensions.Where(x => x.IsActive).ToList();

        public Task<Shared.Entities.Dimension> this[int id] =>
            _dbContext.Dimensions.FirstOrDefaultAsync(x => x.Id == id);

        public async Task<int> AddDimensionAsync(Shared.Entities.Dimension dimension)
        {
            var dbRecord = await _dbContext.Dimensions.FirstOrDefaultAsync(x =>
                 x.Zoom == dimension.Zoom && x.Y == dimension.Y && x.X == dimension.X);
            if (dbRecord == null)
            {
                _dbContext.Dimensions.Add(dimension);
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
            var dimension = await _dbContext.Dimensions.FirstOrDefaultAsync(x => x.Id == dimensionId);
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
            var dimension = await _dbContext.Dimensions.FirstOrDefaultAsync(x => x.Id == dimensionId);
            if (dimension != null)
            {
                dimension.IsActive = false;
                await _dbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<Shared.Entities.Dimension> GetDimension(short zoom, short x, short y, bool autoAdd = true)
        {
            var dimension =
                await _dbContext.Dimensions.FirstOrDefaultAsync(d => d.Zoom == zoom && d.X == x && d.Y == y);
            if (dimension == null && autoAdd)
            {
                dimension = new Shared.Entities.Dimension()
                {
                    Zoom = zoom,
                    X = x,
                    Y = y,
                    IsActive = false
                };
                _dbContext.Dimensions.Add(dimension);
                await _dbContext.SaveChangesAsync();
            }
            return dimension;
        }
    }
}
