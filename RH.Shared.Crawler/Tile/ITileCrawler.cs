using System.IO;
using System.Threading.Tasks;
using RH.EntityFramework.Shared.Entities;

namespace RH.Shared.Crawler.Tile
{
    public interface ITileCrawler:ICrawler
    {
        FileStream GetDimensionContentAsync(EntityFramework.Shared.Entities.Dimension dimension,
            SystemSettings currentSetting);
    }
}
