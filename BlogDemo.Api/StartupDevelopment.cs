using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using BlogDemo.Infrastructure.DateBase;
using BlogDemo.Infrastructure.Repositories;
using BlogDemo.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Logging;
using BlogDemo.Api.Extensions;
using AutoMapper;
using Newtonsoft.Json.Serialization;
using BlogDemo.Infrastructure.Resources;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using BlogDemo.Infrastructure.Services;
using FluentValidation.AspNetCore;
using FluentValidation;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Cors.Internal;

namespace BlogDemo.Api
{
    public class StartupDevelopment
    {
        public IConfiguration configuration { get; set; }
        public StartupDevelopment(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(option => {
                option.ReturnHttpNotAcceptable = true;
              //  option.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                var outputFormatter = option.OutputFormatters.OfType<JsonOutputFormatter>().FirstOrDefault();
                if (outputFormatter != null)
                {
                    outputFormatter.SupportedMediaTypes.Add("application/vnd.cgzl.hateoas+json");
                }

                var intputFormatter = option.InputFormatters.OfType<JsonInputFormatter>().FirstOrDefault();
                if (intputFormatter != null)
                {
                    intputFormatter.SupportedMediaTypes.Add("application/vnd.cgzl.post.create+json");
                    intputFormatter.SupportedMediaTypes.Add("application/vnd.cgzl.post.update+json");
                }

            }).AddJsonOptions(options=> {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }).AddFluentValidation();
            services.AddDbContext<MyContext>(c =>
            {
                c.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
               // c.UseSqlite(configuration["ConnectionStrings:DefaultConnection"]);
                 
            });
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IPostImageRepository, PostImageRepository>();

            
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddHttpsRedirection(options=> {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                options.HttpsPort = 6001;
            });

            //资源（DTO）验证注册
            services.AddTransient<IValidator<PostAddResource>, PostAddOrUpdateResourceValidator<PostAddResource>>();
            services.AddTransient<IValidator<PostUpdateResource>, PostAddOrUpdateResourceValidator<PostUpdateResource>>();
            services.AddTransient<IValidator<PostImageResource>, PostImageResourceValidator >();

            //前一页和后一页url生成
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(factory =>
            {
                var actionContext = factory.GetService<IActionContextAccessor>().ActionContext;
                return new UrlHelper(actionContext);
            });

            //排序
            var propertyMappingContainer = new PropertyMappingContainer();
            propertyMappingContainer.Register<PostPropertyMapping>();
            services.AddSingleton<IPropertyMappingContainer>(propertyMappingContainer);
            //验证Filed是否存在
            services.AddTransient<ITypeHelperService, TypeHelperService>();

            services.AddAutoMapper();

            //授权
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
            .AddIdentityServerAuthentication(options =>
            {
                options.Authority = "https://localhost:5001";
                options.ApiName = "restapi";
            });

            //配置跨域
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularDevOrigin",
                    builder => builder.WithOrigins("http://localhost:4200")
                        .WithExposedHeaders("X-Pagination")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });
            //配置跨域
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularDevOrigin",
                    builder => builder.WithOrigins("http://localhost:4201")
                        .WithExposedHeaders("X-Pagination")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });

            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory("AllowAngularDevOrigin"));//跨域配置
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });


        }


       
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,ILoggerFactory loggerFactory)
        {

            //app.UseDeveloperExceptionPage();//返回错误页面，如果做api就不适合了
            app.UseMyExceptionHandler(loggerFactory);
            app.UseCors("AllowAngularDevOrigin");//跨域配置
            //授权
            app.UseAuthentication();
           
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseMvc();
            
        }
    }
}
