using System.Threading.Tasks;
using RH.Shared.Crawler.Helper;

namespace RH.Shared.Crawler
{
    public interface ICrawler
    {
        Task<CrawlResult> CrawlDimensionContentAsync(EntityFramework.Shared.Entities.Dimension dimension);
    }
}
