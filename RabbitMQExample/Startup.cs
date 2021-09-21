using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using models;
using RabbitMQRevieverExample;

namespace RabbitMQExample
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
          
            services.AddMassTransit(x =>
              {
                  x.UsingAzureServiceBus((context, cfg) =>
                  {
                      cfg.Host("Endpoint=sb://test2service.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=3tnzDHoxSDP/2hsTZGUmNkbLJq42m2SScKvBFUXo/QU=");
                      // Queue implementation
                      cfg.ReceiveEndpoint("test", endpoint =>
                      {
                          endpoint.ConfigureConsumer<MessageConsumer>(context);
                      });
                      // Topic Implementation
                      cfg.SubscriptionEndpoint("test", "topicname", endpoint =>
                      {
                          endpoint.ConfigureConsumer<MessageConsumer>(context);
                      });
                  });
                  x.AddConsumer<MessageConsumer>();
              });
            services.AddMassTransitHostedService(true);
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
