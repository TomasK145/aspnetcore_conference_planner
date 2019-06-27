using FrontEnd.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FrontEnd
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddHttpClient<IApiClient, ApiClient>(client => //konfiguracia ApiClienta
            {
                client.BaseAddress = new Uri(Configuration["serviceUrl"]); //konfiguracna hodnota ulozena v appsettings.json 
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy =>
                {
                    policy.RequireAuthenticatedUser()
                          .RequireIsAdminClaim();
                });
            });

            services.AddMvc()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizeFolder("/Admin", "Admin"); //zabezpecenie folderu
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //definovanie dependency injection
            services.AddSingleton<IAdminService, AdminService>();

            //If you run the app at this point, you'll see an exception stating that you can't inject a scoped type into a type registered as a singleton.
            //This is the DI system protecting you from a common anti - pattern that can arise when using IoC containers. 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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

            //app.UseHttpsRedirection();
            //app.UseCookiePolicy(); //sposobovalo nezobrazovanie info message
            app.UseStaticFiles();
            
            app.UseAuthentication(); //autentifikacny middleware
                        
            app.UseMvc();
        }
    }
}
