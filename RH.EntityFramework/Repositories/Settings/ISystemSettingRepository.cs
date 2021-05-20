using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RH.EntityFramework.Common;
using RH.EntityFramework.Shared.Entities;

namespace RH.EntityFramework.Repositories.Settings
{
    public interface ISystemSettingRepository
    {
        Task<SystemSettings> GetCurrentSetting();
        Task<SystemSettings> this[int id] { get; }
        int ActiveSettingId { get; }
        Task<int> AddDimensionAsync(Shared.Entities.SystemSettings systemSettings);
        Task<PagedList<Shared.Entities.SystemSettings>> GetSystemSettingsAsync(int page = 1, int pageSize = 10);
        Task<EntityFramework.Shared.Entities.SystemSettings> GetActiveSystemSetting();
    }
}
