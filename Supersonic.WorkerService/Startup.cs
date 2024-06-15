using Supersonic.Core.Interfaces;
using Supersonic.Infrastructure.Network;
using Supersonic.WorkerService.Models;

namespace Supersonic.WorkerService
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<NodeConfiguration>(options =>
            {
                options.NodeUrl = Environment.GetEnvironmentVariable("NODE_URL") ?? "http://localhost:5001";
                options.RegistryUrl = Environment.GetEnvironmentVariable("REGISTRY_URL") ?? "http://localhost:5000";
            });

            services.AddSingleton<IConsensusMechanism, ConsensusMechanism>();
            services.AddHttpClient();
            services.AddTransient<NetworkService>();
            services.AddTransient<NodeRegistrar>();
            services.AddHostedService<Worker>();

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseWebSockets();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
