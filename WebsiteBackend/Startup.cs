using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;

namespace WebsiteBackend
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
            var mongodb = Configuration.GetSection("MongoDb").Get<MongoDbSettings>();

            services
                .AddIdentity<ApplicationUser, ApplicationRole>(
                    options => options.SignIn.RequireConfirmedAccount = true)
                .AddMongoDbStores<ApplicationUser, ApplicationRole, BsonObjectId>(
                    mongodb.ConnectionString, mongodb.DatabaseName);
            
            services.AddControllers();
            services.AddSwaggerGen(
                c =>
                {
                    c.SwaggerDoc(
                        "v1",
                        new OpenApiInfo
                        {
                            Title = "WebsiteBackend",
                            Version = "v1"
                        });
                })
                .AddAuthentication()
                .AddPatreon(
                    options =>
                    {
                        IConfigurationSection patreonConfig = Configuration.GetSection("Patreon");
                        
                        options.ClientId = patreonConfig["ClientId"];
                        options.ClientSecret = patreonConfig["ClientSecret"];
                    });
            
            services.ConfigureApplicationCookie(
                options =>
                {
                    options.Events.OnRedirectToAccessDenied = UnauthorizedResponse;
                    options.Events.OnRedirectToLogin = UnauthorizedResponse;
                });
        }
        
        internal static Task UnauthorizedResponse(RedirectContext<CookieAuthenticationOptions> context)
        {
            context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
            return Task.CompletedTask;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebsiteBackend v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}