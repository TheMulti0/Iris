using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDbGenericRepository;
using UserDataLayer;

namespace PollRulesManager
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
            var consumerConfig = Configuration.GetSection<RabbitMqConfig>("ChatPollRequestsConsumer"); 
            var producerConfig = Configuration.GetSection<RabbitMqConfig>("PollRequestsProducer");
            var mongoConfig = Configuration.GetSection<MongoDbConfig>("MongoDb");

            services
                .AddSingleton<IMongoDbContext>(
                    _ => new MongoDbContext(
                        mongoConfig.ConnectionString,
                        mongoConfig.DatabaseName))
                .AddSingleton(mongoConfig)
                .AddSingleton<MongoApplicationDbContext>()
                .AddSingleton<ISavedUsersRepository, MongoSavedUsersRepository>()
                .AddSingleton<IPollRequestsProducer>(
                    provider => new PollRequestsProducer(
                        producerConfig,
                        provider.GetService<ILogger<PollRequestsProducer>>()))
                .AddSingleton<IChatPollRequestsConsumer, ChatPollRequestsConsumer>()
                .AddHostedService(
                    provider => new ChatPollRequestsConsumerService(
                        consumerConfig,
                        provider.GetService<IChatPollRequestsConsumer>(),
                        provider.GetService<ISavedUsersRepository>(),
                        provider.GetService<ILogger<ChatPollRequestsConsumerService>>()));
            
            services.AddControllers();
            services.AddSwaggerGen(
                c =>
                {
                    c.SwaggerDoc(
                        "v1",
                        new OpenApiInfo
                        {
                            Title = "PollRulesManager",
                            Version = "v1"
                        });
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PollRulesManager v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}