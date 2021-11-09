using System.Collections.Generic;
using System.Threading.Tasks;
using RH.EntityFramework.Shared.Entities;

namespace RH.Shared.Crawler.Label
{
    public interface ILabelCrawler:ICrawler
    {
        Task<string> GetDimensionContentAsync(EntityFramework.Shared.Entities.Dimension dimension,
            SystemSettings getCurrentSetting);
        Task<List<EntityFramework.Shared.Entities.Label>> GetDimensionContentAsync(EntityFramework.Shared.Entities.Dimension dimension);
    }
}
