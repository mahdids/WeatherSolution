using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RH.EntityFramework.Repositories.Dimension;
using RH.EntityFramework.Repositories.Forecast.ECMWF;
using RH.EntityFramework.Repositories.Forecast.GFS;
using RH.EntityFramework.Repositories.Label;
using RH.EntityFramework.Shared.DbContexts;
using RH.Services.Worker.Workers;
using RH.Shared.Crawler.Dimension;
using RH.Shared.Crawler.Forecast;
using RH.Shared.Crawler.Label;
using RH.Shared.Crawler.Tile;
using RH.Shared.HttpClient;
using Serilog;

namespace RH.Services.Worker
{
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            //.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .Build();
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)

                .CreateLogger();

            try
            {
                Log.Information("====================================================================");
                Log.Information($"Application Starts. Version: {System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version}");
                Log.Information($"Application Directory: {AppDomain.CurrentDomain.BaseDirectory}");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Application terminated unexpectedly");
            }
            finally
            {
                Log.Information("====================================================================\r\n");
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var databaseType = Configuration["DataBaseType"];
            var connectionString = "";

            var builder = Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .UseSerilog()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddHostedService<GfsWorker>();
                    services.AddHostedService<EcmwfWorker>();


                    switch (databaseType)
                    {
                        case "SqlServer":
                            connectionString = Configuration
                                .GetConnectionString("WindyConnectionString");
                            services.AddDbContext<WeatherDbContext>(options => options.UseSqlServer(connectionString));

                            break;
                        case "MySql":
                            connectionString = Configuration
                                .GetConnectionString("MySqlConnectionString");
                            services.AddDbContext<WeatherDbContext>(options => options.UseMySQL(connectionString));
                            break;
                    }
                    services.AddTransient<IHttpClientFactory, HttpClientFactory>();

                    services.AddTransient<IDimensionRepository, DimensionRepository>();
                    services.AddTransient<ILabelRepository, LabelRepository>();
                    services.AddTransient<IGfsRepository, GfsRepository>();
                    services.AddTransient<IEcmwfRepository, EcmwfRepository>();

                    services.AddTransient<IDimensionManager, DimensionManager>();
                    services.AddTransient<ITileCrawler, WindyTileCrawler>();
                    services.AddTransient<ILabelCrawler, WindyLabelCrawler>();
                    services.AddTransient<WindyGfsCrawler, WindyGfsCrawler>();
                    services.AddTransient<WindyEcmwfCrawler, WindyEcmwfCrawler>();
                });

            return builder;
        }
    }
}
