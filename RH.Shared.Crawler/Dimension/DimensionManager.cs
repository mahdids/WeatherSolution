using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RH.EntityFramework.Repositories.Dimension;
using RH.Shared.Crawler.Helper;

namespace RH.Shared.Crawler.Dimension
{
    public class DimensionManager : IDimensionManager
    {
        private readonly IDimensionRepository _dimensionRepository;
        private readonly ILogger<DimensionManager> _logger;
        private List<EntityFramework.Shared.Entities.Dimension> _dimensions;

        public DimensionManager(IDimensionRepository dimensionRepository, ILogger<DimensionManager> logger)
        {
            _dimensionRepository = dimensionRepository;
            _logger = logger;
        }

        public List<EntityFramework.Shared.Entities.Dimension> Dimensions
        {
            get { return _dimensions ??= _dimensionRepository.GetAllActiveDimensions(); }
            set => _dimensions = value;
        }

        public async Task<DimensionManagerResult> RegenerateAllDimension(short minZoom, short maxZoom, short minZoomX1, short minZoomY1, short minZoomX2, short minZoomY2)
        {
            try
            {
                var zoomDiff = maxZoom - minZoom;
                for (int z = 0; z <= zoomDiff; z++)
                {
                    var pow = (short)Math.Pow(2, z);
                    var newMinX1 = (short)(pow * minZoomX1);
                    var newMinX2 = (short)(pow * (minZoomX2+1));
                    var newMinY1 = (short)(pow * minZoomY1);
                    var newMinY2 = (short)(pow * (minZoomY2+1));
                    for (short x = newMinX1; x < newMinX2; x++)
                        for (short y = newMinY1; y < newMinY2; y++)
                        {
                            var dimension = new EntityFramework.Shared.Entities.Dimension()
                            {
                                IsActive = true,
                                Zoom = (short)(minZoom + z),
                                X = x,
                                Y = y
                            };
                            await _dimensionRepository.AddDimensionAsync(dimension);
                            _logger.LogInformation(
                                $"Generate Dimension Succeeded : {dimension.Zoom}/{dimension.X}/{dimension.Y}");
                        }
                }
                return new DimensionManagerResult() { Succeeded = true };
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"Generate Dimension Exception ");
                return new DimensionManagerResult()
                {
                    Succeeded = false,
                    Exception = e
                };
            }
        }


        public async Task<EntityFramework.Shared.Entities.Dimension> GetDimension(short zoom, short x, short y)
        {
            return await _dimensionRepository.GetDimension(zoom, x, y);
        }

        public void ReloadDimensions()
        {
            _dimensions = null;
        }
    }
}
