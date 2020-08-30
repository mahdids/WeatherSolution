using System.IO;
using System.Threading.Tasks;

namespace RH.Shared.Crawler.Tile
{
    public interface ITileCrawler:ICrawler
    {
        FileStream GetDimensionContentAsync(EntityFramework.Shared.Entities.Dimension dimension);
    }
}
