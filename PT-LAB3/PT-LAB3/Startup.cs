using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using PT_LAB3.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Google;

namespace PT_LAB3
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

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme; 
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddGoogle(options =>
            {
                options.ClientId = "551853330629-hhtbdc9sjsfdhprne98v1tei40085i2b.apps.googleusercontent.com"; 
                options.ClientSecret = "CnB-FWnR4KFzqoustXm9OaUg";
            }).AddCookie(options => 
            { 
                options.Cookie.HttpOnly = true; 
                options.Cookie.SameSite = SameSiteMode.None; 
            });

            services.AddMvc();

            services.AddDbContext<PT_LAB3Context>(options =>
                    options.UseInMemoryDatabase("PT_LAB3Context"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                using(var ss = app.ApplicationServices.CreateScope())                
                { 
                    var context = ss.ServiceProvider.GetService<PT_LAB3Context>(); 
                    context.User.Add(new Models.User { Name = "AAA", Surname = "BBB", EMail = "aaa.bbb@onet.pl" }); 
                    context.SaveChanges(); 
                }
            }
        

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
