using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RH.EntityFramework.Repositories.Dimension;
using RH.EntityFramework.Shared.Entities;
using RH.Shared.Crawler.Helper;

namespace RH.Shared.Crawler.WindDimension
{
    public class WindDimensionManager : IWindDimensionManager
    {
        private readonly IWindDimensionRepository _dimensionRepository;
        private readonly ILogger<WindDimensionManager> _logger;
        private List<EntityFramework.Shared.Entities.WindDimension> _dimensions;

        public WindDimensionManager(IWindDimensionRepository dimensionRepository, ILogger<WindDimensionManager> logger)
        {
            _dimensionRepository = dimensionRepository;
            _logger = logger;
        }

        public List<EntityFramework.Shared.Entities.WindDimension> Dimensions
        {
            get { return _dimensions ??= _dimensionRepository.GetAllActiveDimensions(); }
            set => _dimensions = value;
        }

        public async Task<DimensionManagerResult> RegenerateAllDimension(double intervalX, double intervalY,
            double minX, double minY, double maxX, double maxY)
        {
            try
            {


                for (double x = minX; x < maxX; x += intervalX)
                    for (double y = minY; y < maxY; y += intervalY)
                    {
                        var dimension = new EntityFramework.Shared.Entities.WindDimension()
                        {
                            IsActive = true,
                            X = x,
                            Y = y,
                        };
                        await _dimensionRepository.AddDimensionAsync(dimension);
                        _logger.LogInformation(
                            $"Generate Dimension Succeeded : {dimension.X}/{dimension.Y}");
                    }

                return new DimensionManagerResult() { Succeeded = true };
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"Generate WindDimension Exception ");
                return new DimensionManagerResult()
                {
                    Succeeded = false,
                    Exception = e
                };
            }
        }

        public async Task<EntityFramework.Shared.Entities.WindDimension> GetDimension(double x, double y, bool autoAdd = true)
        {
            return await _dimensionRepository.GetDimension(x, y, autoAdd);
        }

        public void ReloadDimensions()
        {
            _dimensions = null;
        }
        public DimensionBorder GetBorder()
        {
            return _dimensionRepository.GetBorder();
        }
    }
}
