using System.Collections.Generic;
using System.Threading.Tasks;
using RH.Shared.Crawler.Helper;

namespace RH.Shared.Crawler.Dimension
{
    public interface IDimensionManager
    {
         List<EntityFramework.Shared.Entities.Dimension> Dimensions { get; set; }
        Task<DimensionManagerResult> RegenerateAllDimension(short minZoom, short maxZoom, short minZoomX1, short minZoomY1, short minZoomX2, short minZoomY2);
        Task<EntityFramework.Shared.Entities.Dimension> GetDimension(short zoom, short x, short y);

        void ReloadDimensions();
    }
}
