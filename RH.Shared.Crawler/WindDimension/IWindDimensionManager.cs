using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RH.Shared.Crawler.Helper;

namespace RH.Shared.Crawler.WindDimension
{
    public interface IWindDimensionManager
    {
        List<EntityFramework.Shared.Entities.WindDimension> Dimensions { get; set; }
        Task<DimensionManagerResult> RegenerateAllDimension(double intervalX, double intervalY,double minX, double minY, double maxX, double maxY);
        Task<EntityFramework.Shared.Entities.WindDimension> GetDimension(double x,double y, bool autoAdd = true);
        void ReloadDimensions();
    }
}
