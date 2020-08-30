using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace RH.EntityFramework.Shared.DbContexts
{
    class WeatherContextFactory : IDesignTimeDbContextFactory<WeatherDbContext>
    {
        public WeatherDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("AppSettings.json")
                .Build();

            var dbContextBuilder = new DbContextOptionsBuilder();

            var connectionString = configuration
                .GetConnectionString("WindyConnectionString");

            dbContextBuilder.UseSqlServer(connectionString);

            return new WeatherDbContext(dbContextBuilder.Options);
        }
    }
}
