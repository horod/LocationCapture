using System.Linq;
using AutoMapper;
using LocationCapture.BL;
using LocationCapture.DAL;
using LocationCapture.DAL.SqlServer;
using LocationCapture.Models;
using LocationCapture.WebAPI.Models;
using LocationCapture.WebAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LocationCapture.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            OptionsConfigurationServiceCollectionExtensions.Configure<AppSettings>(services, Configuration.GetSection("AppSettings"));
            services.AddTransient<ILocationContextFactory, LocationContextFactory>();
            services.AddTransient<ILocationDataService, LocationDataService>();
            services.AddTransient<ILocationSnapshotDataService, LocationSnapshotDataService>();
            services.AddTransient<IFileSystemService, FileSystemService>();
            services.AddTransient<IImageService, ImageService>();

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Location, LocationDto>().ForMember(dest => dest.SnapshotsCount, opt => opt.MapFrom(src => src.LocationSnapshots.Count()));
                cfg.CreateMap<LocationForCreationDto, Location>();
                cfg.CreateMap<LocationForUpdateDto, Location>();
                cfg.CreateMap<LocationSnapshot, LocationSnapshotDto>();
                cfg.CreateMap<LocationSnapshotForCreationDto, LocationSnapshot>();
                cfg.CreateMap<LocationSnapshotForUpdateDto, LocationSnapshot>();
                cfg.CreateMap<SnapshotGroup, SnapshotGroupDto>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
