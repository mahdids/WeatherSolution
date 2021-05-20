using System.Threading.Tasks;
using RH.EntityFramework.Shared.Entities;

namespace RH.Shared.Crawler.Label
{
    public interface ILabelCrawler:ICrawler
    {
        Task<string> GetDimensionContentAsync(EntityFramework.Shared.Entities.Dimension dimension,
            SystemSettings getCurrentSetting);
    }
}
