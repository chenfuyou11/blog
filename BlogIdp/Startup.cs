// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using BlogDemo.Core.Security;
using BlogDemo.Core.Security.Permissions;
using BlogDemo.Core.Security.Services;
using BlogIdp.Data;
using BlogIdp.Models;
using BlogIdp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;

namespace BlogIdp
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }
      


        public Startup(IConfiguration configuration, IHostingEnvironment environment )
        {
            Configuration = configuration;
            Environment = environment;
           
        }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddScoped<UserManager<ApplicationUser>>();
            services.AddScoped<RoleManager<ApplicationRole>>();
            services.AddScoped<IUserService, UserService>();
            //注入授权Handler
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthorization(options =>
            {
                //自定义Requirement，userPermission可从数据库中获得
                var rolePermission = new List<RolePermission> {
                              //new UserPermission {  Url="/", UserName="gsw"},
                              new RolePermission {  Url="/api/user", RoleName="Administrator"},
                          };

                //var roleMgr = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                //var name=   roleMgr.FindByNameAsync("Administrator");



                //using (var scope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                //{
                //    var _roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
                //    //赋值用户权限
                //    var role = _roleManager.FindByNameAsync("Administrator");
                //}

               


                options.AddPolicy("Permission",
                   policy => policy.Requirements.Add(new PermissionRequirement("/denied", rolePermission)));
            });


            services.AddMvc();

            services.Configure<IISOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApis())
                .AddInMemoryClients(Config.GetClients())
                .AddAspNetIdentity<ApplicationUser>();

            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                throw new Exception("need to configure key material");
            }


            //https配置
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
                // options.ExcludedHosts.Add("example.com");
                // options.ExcludedHosts.Add("www.example.com");
            });

            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                options.HttpsPort = 5001;
            });


            services.AddCors(options =>
            {
                //options.AddPolicy("AngularDev", policy =>
                //{
                //    policy.WithOrigins("http://localhost:4200")
                //        .AllowAnyHeader()
                //        .AllowAnyMethod();
                //});
                options.AddPolicy("AngularDev", policy =>
                {
                    policy.WithOrigins("http://localhost:4201")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            

        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseCors();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseMvcWithDefaultRoute();
        }
    }
}
