using Microsoft.EntityFrameworkCore;
using RH.EntityFramework.Shared.Entities;

namespace RH.EntityFramework.Shared.DbContexts
{
    public class WeatherDbContext: DbContext
    {

        public DbSet<Dimension> Dimensions { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<Forecast> Forecasts { get; set; }
        public DbSet<Gfs> Gfses{ get; set; }
        public DbSet<Ecmwf> Ecmwfs{ get; set; }
        public DbSet<WindyTime> WindyTimes { get; set; }



        public WeatherDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
