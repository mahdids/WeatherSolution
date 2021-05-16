using Microsoft.EntityFrameworkCore;
using RH.EntityFramework.Shared.Entities;

namespace RH.EntityFramework.Shared.DbContexts
{
    public class WeatherDbContext: DbContext
    {

        public DbSet<Dimension> Dimensions { get; set; }
        public DbSet<WindDimension> WindDimensions { get; set; }
        public DbSet<GfsForecast> GfsForecasts { get; set; }
        public DbSet<EcmwfForecast> EcmwfForecasts { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<Gfs> Gfses{ get; set; }
        public DbSet<Ecmwf> Ecmwfs{ get; set; }
        public DbSet<WindyTime> WindyTimes { get; set; }

        public DbSet<SystemSettings> SystemSettings { get; set; }



        public WeatherDbContext(DbContextOptions options) : base(options)
        {
            
        }
    }
}
