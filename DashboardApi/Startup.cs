using System.Net;
using System.Threading.Tasks;
using AspNetCore.Identity.MongoDbCore.Models;
using Common;
using Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDbGenericRepository;
using Tweetinvi.Models;
using UpdatesDb;
using MongoDbConfig = Common.MongoDbConfig;

namespace DashboardApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) => Configuration = configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var twitterCredentials = Configuration.GetSection<TwitterCredentials>("TwitterCredentials");

            services.AddSingleton(twitterCredentials);

            services.AddUpdatesDb()
                .AddIdentity<ApplicationUser, MongoIdentityRole<ObjectId>>(
                    options => options.SignIn.RequireConfirmedAccount = true)
                .AddMongoDbStores<ApplicationUser, MongoIdentityRole<ObjectId>, ObjectId>(
                    CreateMongoDbContext(Configuration.GetSection<MongoDbConfig>("IdentityDb")));

            services.AddCors(
                options =>
                {
                    options.AddPolicy(
                        "MyPolicy",
                        b => b.WithOrigins("http://localhost:4200")
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials());
                });

            services.AddAuthentication()
                .AddTwitter(
                    options =>
                    {
                        options.SaveTokens = true;

                        options.ConsumerKey = twitterCredentials.ConsumerKey;
                        options.ConsumerSecret = twitterCredentials.ConsumerSecret;
                    });

            services.ConfigureApplicationCookie(
                options =>
                {
                    options.Events.OnRedirectToAccessDenied = UnauthorizedResponse;
                    options.Events.OnRedirectToLogin = UnauthorizedResponse;
                });
            
            services.AddControllers()
                .AddJsonOptions(
                    options =>
                    {
                        options.JsonSerializerOptions.Converters.Add(new MediaJsonConverter());
                        options.JsonSerializerOptions.Converters.Add(new TimeSpanConverter());
                        options.JsonSerializerOptions.Converters.Add(new NullableTimeSpanConverter());
                    });

            services.AddSwaggerGen(
                c =>
                {
                    c.SwaggerDoc(
                        "v1",
                        new OpenApiInfo
                        {
                            Title = "DashboardApi",
                            Version = "v1"
                        });
                });
        }

        private static Task UnauthorizedResponse(RedirectContext<CookieAuthenticationOptions> context)
        {
            context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
            return Task.CompletedTask;
        }

        private IMongoDbContext CreateMongoDbContext(MongoDbConfig mongoDbConfig)
            => new MongoDbContext(mongoDbConfig.ConnectionString, mongoDbConfig.DatabaseName);

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DashboardApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseCors("MyPolicy");

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllerRoute(
                        "default",
                        "{controller}/{action=Index}/{id?}");
                });

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}