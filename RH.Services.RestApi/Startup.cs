using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RH.EntityFramework.Repositories.Dimension;
using RH.EntityFramework.Repositories.Forecast.ECMWF;
using RH.EntityFramework.Repositories.Forecast.GFS;
using RH.EntityFramework.Repositories.Label;
using RH.EntityFramework.Repositories.Settings;
using RH.EntityFramework.Repositories.Wind;
using RH.EntityFramework.Shared.DbContexts;
using RH.Shared.Crawler.Dimension;
using RH.Shared.Crawler.Forecast;
using RH.Shared.Crawler.Forecast.CityTile;
using RH.Shared.Crawler.Forecast.Wind;
using RH.Shared.Crawler.Label;
using RH.Shared.Crawler.Tile;
using RH.Shared.Crawler.WindDimension;
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

            var databaseType = Configuration["DataBaseType"];
            var connectionString = "";
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
                    services.AddDbContext<WeatherDbContext>(options =>
                        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
                    break;
            }

            services.AddTransient<IHttpClientFactory, HttpClientFactory>();

            services.AddTransient<ISystemSettingRepository, SystemSettingRepository>();
            services.AddTransient<IDimensionRepository, DimensionRepository>();
            services.AddTransient<IWindDimensionRepository, WindDimensionRepository>();
            services.AddTransient<ILabelRepository, LabelRepository>();
            services.AddTransient<IGfsRepository, GfsRepository>();
            services.AddTransient<IEcmwfRepository, EcmwfRepository>();
            services.AddTransient<GfsWindRepository, GfsWindRepository>();
            services.AddTransient<EcmwfWindRepository, EcmwfWindRepository>();

            services.AddTransient<IDimensionManager, DimensionManager>();
            services.AddTransient<IWindDimensionManager, WindDimensionManager>();
            services.AddTransient<ITileCrawler, WindyTileCrawler>();
            services.AddTransient<ILabelCrawler, WindyLabelCrawler>();
            services.AddTransient<GfsCityTileCrawler, GfsCityTileCrawler>();
            services.AddTransient<GfsWindCrawler, GfsWindCrawler>();
            services.AddTransient<EcmwfCityTileCrawler, EcmwfCityTileCrawler>();
            services.AddTransient<EcmwfWindCrawler, EcmwfWindCrawler>();
            //services.AddSwaggerGen();
            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "RH.Services.RestApi",
                //Version = "v1",
                //Description = "An API to perform Employee operations",
                //TermsOfService = new Uri("https://example.com/terms"),
                Contact = new OpenApiContact
                {
                    Name = "System Setting",
                    //Email = "John.Walkner@gmail.com",
                    //Url = new Uri(c.SwaggerGeneratorOptions.Servers.FirstOrDefault().Url+"/Setting")
                },
                //License = new OpenApiLicense
                //{
                //    Name = "Employee API LICX",
                //    Url = new Uri("https://example.com/license"),
                //}
            }));


            services.AddMvc(option => option.EnableEndpointRouting = true);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseMvcWithDefaultRoute();
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Weather Service");
                c.RoutePrefix = "Service";
            });
            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}");
                //endpoints.MapDefaultControllerRoute();
            });


        }
    }
}
