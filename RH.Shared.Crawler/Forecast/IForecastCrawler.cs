using System.Threading.Tasks;

namespace RH.Shared.Crawler.Forecast
{
    public interface  IForecastCrawler:ICrawler
    {
        Task<string> GetDimensionContentAsync(EntityFramework.Shared.Entities.Dimension dimension);
    }
}
