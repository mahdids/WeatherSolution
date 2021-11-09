using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RH.EntityFramework.Shared.Entities;

namespace RH.Services.RestApi.Models
{
    public class CityTileWebViewModel
    {
        public CityTileWebViewModel()
        {
            CityTiles = new List<CityTile>();
        }
        public bool HasData { get; set; }

        public List<CityTile> CityTiles { get; set; }
    }
}
