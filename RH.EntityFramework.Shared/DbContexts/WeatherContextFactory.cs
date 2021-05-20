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

            var databaseType = configuration["DataBaseType"];
            var dbContextBuilder = new DbContextOptionsBuilder();
            var connectionString = "";
            switch (databaseType)
            {
                case "SqlServer":
                    connectionString = configuration
                        .GetConnectionString("WindyConnectionString");
                    dbContextBuilder.UseSqlServer(connectionString);
                    break;
                case "MySql":
                    connectionString = configuration
                        .GetConnectionString("MySqlConnectionString");
                    dbContextBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

                    break;
            }
            return new WeatherDbContext(dbContextBuilder.Options);
        }
    }
}
