using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RH.EntityFramework.Shared.Entities;

namespace RH.Shared.Crawler.Forecast
{
    public interface  IForecastCrawler:ICrawler
    {
        Task<string> GetDimensionContentAsync(EntityFramework.Shared.Entities.Dimension dimension, SystemSettings currentSetting);
        Task<string> GetDimensionContentByTimeAsync(EntityFramework.Shared.Entities.Dimension dimension, long epocTime);
        Task<List<EntityFramework.Shared.Entities.CityTile>> GetDimensionContentByTimeAsync(EntityFramework.Shared.Entities.Dimension dimension, DateTime date);
    }
}
