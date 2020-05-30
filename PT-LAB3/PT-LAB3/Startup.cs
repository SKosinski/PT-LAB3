using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
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
using Microsoft.AspNetCore.Authorization;
using Google.Api;
using System.Security.Claims; 

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

            services.AddDbContext<PT_LAB3Context>(options =>
                options.UseSqlServer(Configuration["ConnectionStrings:PTLAB3stanislawk171978"]));

            // PODP4.1
            /*
             * Aby skorzysta� z tego rozwi�zania w postmanie
             * nale�y do postmanowego requesta doda� cookie
             * zrobiony wcze�niej w przegl�darce
             * �r�d�o rozwi�zania: https://community.postman.com/t/how-to-test-asp-net-core-web-api-with-cookie-authentication-using-postman/4809
             */
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme; 
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddGoogle(options =>
            {
                options.ClientId = Configuration["Authentication:Google:ClientId"];
                options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
            }).AddCookie(options => 
            { 
                options.Cookie.HttpOnly = false; 
                options.Cookie.SameSite = SameSiteMode.None; 
            });

            // PODP6
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder =>
                    {
                        builder.WithOrigins("https://localhost:44314/");
                    });
            });


            services.AddAuthorization(options =>
                options.AddPolicy("User", policy =>
                    policy.Requirements.Add(new UserPolicyRequirement())));

            services.AddSingleton<IAuthorizationHandler, UserPolicyHandler>();

            services.AddMvc();
            
        }

        public class UserPolicyHandler : AuthorizationHandler<UserPolicyRequirement>
        {
            protected override Task HandleRequirementAsync(
                AuthorizationHandlerContext context,
                UserPolicyRequirement requirement)
            {
                var user = context.User.Claims.ToList();
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
        }

        public class UserPolicyRequirement : IAuthorizationRequirement { }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
