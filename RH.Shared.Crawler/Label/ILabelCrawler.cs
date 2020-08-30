using System.Threading.Tasks;

namespace RH.Shared.Crawler.Label
{
    public interface ILabelCrawler:ICrawler
    {
        Task<string> GetDimensionContentAsync(EntityFramework.Shared.Entities.Dimension dimension);
    }
}
