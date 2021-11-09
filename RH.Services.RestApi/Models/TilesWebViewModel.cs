using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RH.EntityFramework.Shared.Entities;

namespace RH.Services.RestApi.Models
{
    public class TilesViewModel
    {
        public TilesViewModel()
        {
            Labels = new List<Label>();
        }

        public bool HasData { get; set; }

        public string Src { get; set; }

        public List<Label> Labels { get; set; }
    }
}
