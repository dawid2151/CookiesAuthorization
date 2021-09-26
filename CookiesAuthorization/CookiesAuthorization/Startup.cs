using System;
using CookiesAuthorization.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CookiesAuthorization
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
            services.AddControllers();
            services.AddCors();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.LoginPath = "/users/login";
                    options.AccessDeniedPath = "/users/login";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                    options.Cookie.Name = "Auth";
                    options.Cookie.HttpOnly = false;
                    options.Cookie.IsEssential = true;
                });
            services.AddSingleton<IDatabaseProvider, MockDatabaseProvider>();
            services.AddSingleton<IUsersService, UsersService>();
            services.AddSingleton<IHashingService, SHA256HashingService>();
            services.AddSingleton<IUserMapper, UserMapper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(policy =>
            {
                policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            });
            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None,
                CheckConsentNeeded = x => false,
                HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.None
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
