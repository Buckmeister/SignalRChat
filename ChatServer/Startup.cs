using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ChatServer.Hubs;
using System.Collections.Generic;

namespace ChatServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR()
            .AddHubOptions<MessageHub>(options =>
            {
                // 1MB Message Buffer
                options.MaximumReceiveMessageSize = 1024 * 1024;
                options.StreamBufferCapacity = 100;
            })
            .AddMessagePackProtocol(options =>
            {
                options.FormatterResolvers = new List<MessagePack.IFormatterResolver>()
                {
                    MessagePack.Resolvers.StandardResolver.Instance
                };
            });

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<IHubConnectionEventLog, HubConnectionEventLog>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub( options =>
                {
                    // 1MB Message Buffer
                    options.ApplicationMaxBufferSize = 1024 * 1024;
                    options.TransportMaxBufferSize = 1024 * 1024;

                });
                endpoints.MapFallbackToPage("/_Host");
                endpoints.MapHub<MessageHub>("/Chat", options =>
                {
                    // 1MB Message Buffer
                    options.ApplicationMaxBufferSize = 1024 * 1024;
                    options.TransportMaxBufferSize = 1024 * 1024;
                });
            });
        }
    }
}
