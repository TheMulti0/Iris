using System.Text.Json.Serialization;
using Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PostsListener.Client;
using Scraper.MassTransit.Client;
using Scraper.MassTransit.Common;
using SubscriptionsDb;

namespace Dashboard
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
            services.AddCors(options =>
            {
                options.AddPolicy("All",
                                  builder =>
                                  {
                                      builder
                                          .AllowAnyOrigin()
                                          .AllowAnyMethod()
                                          .AllowAnyHeader();
                                  });
            });
            
            services.AddControllers().AddJsonOptions(ConfigureJson);
            services.AddSwaggerGen(
                c =>
                {
                    c.SwaggerDoc(
                        "v1",
                        new OpenApiInfo
                        {
                            Title = "Dashboard",
                            Version = "v1"
                        });
                });
            
            var connectionConfig = Configuration.GetSection("RabbitMqConnection").Get<RabbitMqConfig>();

            services
                .AddSubscriptionsDb()
                .AddScraperMassTransitClient()
                .AddMassTransit(
                    connectionConfig,
                    x => x.AddPostsListenerClient());
        }

        private void ConfigureJson(JsonOptions options)
        {
            options.JsonSerializerOptions.Converters.Add(new TimeSpanConverter());
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.Converters.Add(new BsonObjectIdConverter());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("All");
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dashboard v1"));
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}