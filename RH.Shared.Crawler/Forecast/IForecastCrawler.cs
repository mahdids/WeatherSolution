using System.Threading.Tasks;
using RH.EntityFramework.Shared.Entities;

namespace RH.Shared.Crawler.Forecast
{
    public interface  IForecastCrawler:ICrawler
    {
        Task<string> GetDimensionContentAsync(EntityFramework.Shared.Entities.Dimension dimension, SystemSettings currentSetting);
        Task<string> GetDimensionContentByTimeAsync(EntityFramework.Shared.Entities.Dimension dimension, long epocTime);
    }
}
