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

        //private AuthorizationHandlerContext _aContext;

        //public static void RequestAuthorizationHandlerContext (AuthorizationHandlerContext context)
        //{
        //    _aContext = context;
        //}

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<PT_LAB3Context>(options =>
                    options.UseInMemoryDatabase("PT_LAB3Context"));

            // PODP4.1
            /*
             * Aby skorzystaæ z tego rozwi¹zania w postmanie
             * nale¿y do postmanowego requesta dodaæ cookie
             * zrobiony wczeœniej w przegl¹darce
             * Ÿród³o rozwi¹zania: https://community.postman.com/t/how-to-test-asp-net-core-web-api-with-cookie-authentication-using-postman/4809
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

        //public class AdminPolicyHandler : AuthorizationHandler<AdminPolicyRequirement>
        //{
        //    const string GoogleEmailAddressSchema = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
        //    protected override Task HandleRequirementAsync(
        //        AuthorizationHandlerContext context,
        //        AdminPolicyRequirement requirement)
        //    {
        //        var email = context.User.Claims.FirstOrDefault(p =>
        //            p.Issuer.Equals("Google") &&
        //            p.Type.Equals(GoogleEmailAddressSchema));
        //        if (email != null && email.Value.Equals("stanislaw.j.kosinski@gmail.com"))
        //            context.Succeed(requirement);
        //        return Task.CompletedTask;
        //    }
        //}

        //public class AdminPolicyRequirement : IAuthorizationRequirement { }
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
                //using(var ss = app.ApplicationServices.CreateScope())                
                //{ 
                //    var context = ss.ServiceProvider.GetService<PT_LAB3Context>();

                //    //const string GoogleEmailAddressSchema = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";

                //    //var email = _aContext.User.Claims.FirstOrDefault(p =>
                //    //    p.Issuer.Equals("Google") &&
                //    //    p.Type.Equals(GoogleEmailAddressSchema));
                //    //var name = _aContext.User.Claims.ToList();
                //    ////var surname

                //    //if (email != null && email != null && email != null)

                //    context.User.Add(new Models.User { Name = "AAA", Surname = "BBB", EMail = "aaa.bbb@onet.pl" }); 
                //    context.SaveChanges(); 
                //}
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
            //app.UseMvc();
        }
    }
}
