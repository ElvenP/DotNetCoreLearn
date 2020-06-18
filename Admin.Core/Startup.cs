using Admin.Core.Common.Aop;
using Admin.Core.Common.Attributes;
using Admin.Core.Common.Configs;
using Admin.Core.Common.Helpers;
using Admin.Core.Db;
using Admin.Core.Enums;
using Autofac;
using Autofac.Extras.DynamicProxy;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FreeSql;

namespace Admin.Core
{
    public class Startup
    {
        private readonly AppConfig _appConfig;
        private readonly ConfigHelper _configHelper;
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _env;


        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
            _configHelper = new ConfigHelper();
            _appConfig = _configHelper.Get<AppConfig>("appconfig", env.EnvironmentName) ?? new AppConfig();
        }

        private static string BasePath => AppContext.BaseDirectory;

        public void ConfigureServices(IServiceCollection services)
        {
            //数据库
           // services.AddDb(_env, _appConfig);

            var serviceAssembly = Assembly.Load("Admin.Core.Service");
            services.AddAutoMapper(serviceAssembly);
            services.AddControllers();
           

            #region Swagger Api文档

            if (_env.IsDevelopment() || _appConfig.Swagger)
                services.AddSwaggerGen(c =>
                {
                    typeof(ApiVersion).GetEnumNames().ToList().ForEach(version =>
                    {
                        c.SwaggerDoc(version, new OpenApiInfo
                        {
                            Version = version,
                            Title = "Admin.Core"
                        });
                        //c.OrderActionsBy(o => o.RelativePath);
                    });

                    var xmlPath = Path.Combine(BasePath, "Admin.Core.xml");
                    c.IncludeXmlComments(xmlPath, true);

                    var xmlCommonPath = Path.Combine(BasePath, "Admin.Core.Common.xml");
                    c.IncludeXmlComments(xmlCommonPath, true);

                    var xmlModelPath = Path.Combine(BasePath, "Admin.Core.Model.xml");
                    c.IncludeXmlComments(xmlModelPath);

                    var xmlServicesPath = Path.Combine(BasePath, "Admin.Core.Service.xml");
                    c.IncludeXmlComments(xmlServicesPath);

                    //添加设置Token的按钮
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = "Value: Bearer {token}",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                    });

                    //添加Jwt验证设置
                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                },
                                Scheme = "oauth2",
                                Name = "Bearer",
                                In = ParameterLocation.Header
                            },
                            new List<string>()
                        }
                    });
                });

            #endregion
        }


        public void ConfigureContainer(ContainerBuilder builder)
        {
            #region AutoFac IOC容器

            try
            {
                var dbConfig = new ConfigHelper().Get<DbConfig>("dbconfig", _env.EnvironmentName);
                var freeSqlBuilder = new FreeSqlBuilder()
                    .UseConnectionString(dbConfig.Type, dbConfig.ConnectionString)
                    .UseAutoSyncStructure(dbConfig.SyncStructure)
                    .UseLazyLoading(false)
                    .UseNoneCommandParameter(true);
                var fsql = freeSqlBuilder.Build();

                builder.RegisterInstance(fsql).SingleInstance();
                builder.RegisterType(typeof(UnitOfWorkManager)).InstancePerLifetimeScope();
                #region SingleInstance

                //无接口注入单例
                var assemblyCore = Assembly.Load("Admin.Core");
                var assemblyCommon = Assembly.Load("Admin.Core.Common");
                builder.RegisterAssemblyTypes(assemblyCore, assemblyCommon)
                    .Where(t => t.GetCustomAttribute<SingleInstanceAttribute>() != null)
                    .SingleInstance();
                //有接口注入单例
                builder.RegisterAssemblyTypes(assemblyCore, assemblyCommon)
                    .Where(t => t.GetCustomAttribute<SingleInstanceAttribute>() != null)
                    .AsImplementedInterfaces()
                    .SingleInstance();

                #endregion

                #region Aop

                var interceptorServiceTypes = new List<Type>();
                if (_appConfig.Aop.Transaction)
                {
                    builder.RegisterType<TransactionInterceptor>();
                    interceptorServiceTypes.Add(typeof(TransactionInterceptor));
                }

                #endregion

                #region Repository

                var assemblyRepository = Assembly.Load("Admin.Core.Repository");
                builder.RegisterAssemblyTypes(assemblyRepository)
                    .AsImplementedInterfaces()
                    .InstancePerDependency();

                #endregion

                #region Service

                var assemblyServices = Assembly.Load("Admin.Core.Service");
                builder.RegisterAssemblyTypes(assemblyServices)
                    .AsImplementedInterfaces()
                    .InstancePerDependency()
                    .EnableInterfaceInterceptors()
                    .InterceptedBy(interceptorServiceTypes.ToArray());

                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\n" + ex.InnerException);
            }

            #endregion
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseRouting();

            #region Swagger Api文档

            if (!_env.IsDevelopment() && !_appConfig.Swagger) return;
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                typeof(ApiVersion).GetEnumNames().OrderByDescending(e => e).ToList().ForEach(version =>
                {
                    c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"Admin.Core {version}");
                });
                c.RoutePrefix = ""; //直接根目录访问
                c.DocExpansion(DocExpansion.None); //折叠Api
                //c.DefaultModelsExpandDepth(-1);//不显示Models
            });

            #endregion

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

          
        }
    }
}