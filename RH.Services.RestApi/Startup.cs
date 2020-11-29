using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using RH.EntityFramework.Repositories.Dimension;
using RH.EntityFramework.Repositories.Forecast.ECMWF;
using RH.EntityFramework.Repositories.Forecast.GFS;
using RH.EntityFramework.Repositories.Label;
using RH.EntityFramework.Shared.DbContexts;
using RH.Shared.Crawler.Dimension;
using RH.Shared.Crawler.Forecast;
using RH.Shared.Crawler.Label;
using RH.Shared.Crawler.Tile;
using RH.Shared.HttpClient;

namespace RH.Services.RestApi
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment HostEnvironment { get; }
        public Startup(IConfiguration configuration, IWebHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            HostEnvironment = hostEnvironment;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            string connectionString = Configuration.GetConnectionString("WindyConnectionString");
            services.AddDbContext<WeatherDbContext>(options => options.UseSqlServer(connectionString));

            //services.AddDbContextPool<WeatherDbContext>(
            //    dbContextOptions => dbContextOptions
            //        .UseMySql(
            //            // Replace with your connection string.
            //            "server=localhost;user=root;password=123ewqasdcxz;database=WeatherTest1",
            //            // Replace with your server version and type.
            //            // For common usages, see pull request #1233.
            //            mySqlOptions => mySqlOptions
            //                .CharSetBehavior(CharSetBehavior.NeverAppend))
            //        // Everything from this point on is optional but helps with debugging.
            //        .EnableSensitiveDataLogging()
            //        .EnableDetailedErrors()
            //);
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

            services.AddSwaggerGen();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Weather Service");
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
