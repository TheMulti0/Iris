using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using DashboardBackend.Controllers;
using DashboardBackend.Data;
using DashboardBackend.Models;
using Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using UpdatesConsumer;

namespace DashboardBackend
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
            services.AddDbContext<ApplicationDbContext>(
                options =>
                    options.UseSqlite(
                        Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            var twitter = Configuration.GetSection("Authentication:Twitter").Get<TwitterSettings>();
            services.AddSingleton(twitter);
            services.AddAuthentication()
                .AddTwitter(
                    options =>
                    {
                        options.SaveTokens = true;

                        options.ConsumerKey = twitter.ConsumerKey;
                        options.ConsumerSecret = twitter.ConsumerSecret;
                    });

            services.ConfigureApplicationCookie(
                options =>
                {
                    options.Events.OnRedirectToAccessDenied = UnauthorizedResponse;
                    options.Events.OnRedirectToLogin = UnauthorizedResponse;
                });
            
            var updatesConsumerConfig = Configuration
                .GetSection("UpdatesConsumer").Get<ConsumerConfig>();
            
            services.AddConsumer<string, Update>(
                updatesConsumerConfig,
                new JsonSerializerOptions
                {
                    Converters =
                    {
                        new MediaJsonSerializer()
                    }
                })
                .AddSingleton<IUpdateConsumer, UpdatesDataLayerAppender>()
                .AddHostedService<UpdatesConsumerService>();

            services.AddCors(options =>
            {
                options.AddPolicy(
                    "CorsPolicy",
                    builder => builder.WithOrigins("http://localhost:4200")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            services.AddSingleton(
                Configuration.GetSection("Roles").Get<RolesSettings>());

            services.AddControllers();
        }
        
        internal static Task UnauthorizedResponse(RedirectContext<CookieAuthenticationOptions> context)
        {
            context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
            return Task.CompletedTask;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseCors("CorsPolicy");
            
            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller}/{action=Index}/{id?}");
                });

            CreateRoles(serviceProvider).Wait();
        }

        private static async Task CreateRoles(IServiceProvider provider)
        {
            var roleManager = provider.GetService<RoleManager<IdentityRole>>();
            
            await roleManager.CreateAsync(new IdentityRole(RoleNames.SuperUser));
        }
    }
}