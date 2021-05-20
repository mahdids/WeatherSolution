using System;
using Microsoft.EntityFrameworkCore;

namespace RH.EntityFramework.Shared.Entities
{
    public class SystemSettings
    {
        public int Id { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? EndDate { get; set; }
        public int CrawlingInterval { get; set; }

        public BaseWorkerSetting BaseWorkerSetting { get; set; }

        public DimensionsSetting Dimension { get; set; }

        public WindDimensionsSetting WindDimensions { get; set; }

        public CrawlWebPath CrawlWebPath { get; set; }
    }

    [Owned]
    public class CrawlWebPath
    {
        public string ForecastCityTileGFS { get; set; }
        public string ForecastCityTileECMWF { get; set; }
        public string ForecastWindGFS { get; set; }
        public string ForecastWindECMWF { get; set; }
        public string LabelPath { get; set; }
        public string TileWebPath { get; set; }
        public string TileDirectoryPath { get; set; }


    }
    [Owned]
    public class WindDimensionsSetting
    {
        public int XInterval { get; set; }
        public int YInterval { get; set; }

        public SettingPoint TopLeft { get; set; }
        public SettingPoint BottomRight { get; set; }
    }
    [Owned]
    public class DimensionsSetting
    {
        public int MinZoom { get; set; }
        public int MaxZoom { get; set; }
        public SettingPoint TopLeft { get; set; }
        public SettingPoint BottomRight { get; set; }
    }
    [Owned]
    public class SettingPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
    [Owned]
    public class BaseWorkerSetting
    {
        public bool RegenerateDimension { get; set; }
        public bool RegenerateWindDimension { get; set; }
        public bool ReCrawlLabel { get; set; }
        public bool ReCrawlTileImage { get; set; }
    }
}

