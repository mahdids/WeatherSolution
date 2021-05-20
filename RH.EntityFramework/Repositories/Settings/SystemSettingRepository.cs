using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RH.EntityFramework.Common;
using RH.EntityFramework.Shared.DbContexts;
using RH.EntityFramework.Shared.Entities;

namespace RH.EntityFramework.Repositories.Settings
{
    public class SystemSettingRepository : ISystemSettingRepository
    {
        private readonly WeatherDbContext _dbContext;
        private readonly ILogger<SystemSettingRepository> _logger;
        private SystemSettings _currentSetting;

        public SystemSettingRepository(WeatherDbContext dbContext, ILogger<SystemSettingRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }



        public async Task<SystemSettings> GetCurrentSetting()
        {
            var activeId = ActiveSettingId;
            if (_currentSetting == null || _currentSetting.Id != activeId)
                _currentSetting = await this[activeId];
            return _currentSetting;
        }

        public Task<SystemSettings> this[int id] => _dbContext.SystemSettings.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        public int ActiveSettingId => _dbContext.SystemSettings.AsNoTracking().FirstOrDefault(x => x.IsActive).Id;

        public async Task<int> AddDimensionAsync(SystemSettings systemSetting)
        {
            try
            {
                var old = await _dbContext.SystemSettings.FirstOrDefaultAsync(x => x.IsActive);
                old.EndDate = DateTime.Now;
                old.IsActive = false;

                systemSetting.IsActive = true;
                systemSetting.CreateDate = DateTime.Now;
                await _dbContext.SystemSettings.AddAsync(systemSetting);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Deactivate Setting With Id:{old.Id}, Add New Setting With Id:{systemSetting.Id}");
                return systemSetting.Id;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Add Setting Error");
                return 0;
            }
        }

        public async Task<PagedList<SystemSettings>> GetSystemSettingsAsync(int page = 1, int pageSize = 10)
        {
            return await PagedList<SystemSettings>.ToPagedList(_dbContext.SystemSettings.OrderByDescending(x => x.Id), page, pageSize);
        }

        public async Task<SystemSettings> GetActiveSystemSetting()
        {
            return await _dbContext.SystemSettings.FirstOrDefaultAsync(x => x.IsActive);
        }
    }
}
