using System;
using SampleDataCollector.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StatsdClient;
using System.Threading.Tasks;

namespace SampleDataCollector
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var place = Environment.GetEnvironmentVariable("ENDPOINT") ?? "127.0.0.1";
            var svcVersion = Environment.GetEnvironmentVariable("SVC_VERSION") ?? "default";
            var dogstatsdConfig = new StatsdConfig
            {
                StatsdServerName = place,
                StatsdPort = 8125, // Optional; default is 8125
                Prefix = $"myApp{svcVersion}" // Optional; by default no prefix will be prepended
            };

            StatsdClient.DogStatsd.Configure(dogstatsdConfig);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddLogging();
            services.AddSingleton<IHostedService, DataCollector>();
            services.AddTransient<IDumbThing, DumbThing>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
