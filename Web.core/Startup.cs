using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Admin.Core.Common.Aop;
using Admin.Core.Common.Attributes;
using Admin.Core.Common.Auth;
using Admin.Core.Common.Cache;
using Admin.Core.Common.Configs;
using Admin.Core.Common.Helpers;
using Autofac;
using Autofac.Extras.DynamicProxy;
using AutoMapper;
using FreeSql;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using Web.core.Auth;
using Web.core.Enums;
using MicrosoftMemoryCache = Microsoft.Extensions.Caching.Memory;
using MemoryCache = Admin.Core.Common.Cache.MemoryCache;


namespace Web.core
{
    public class Startup
    {
        private readonly AppConfig _appConfig;

        private readonly ConfigHelper _configHelper;

        //private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _env;


        public Startup(IWebHostEnvironment env)
        {
            //_configuration = configuration;
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
                            Title = "Web.Core"
                        });
                        //c.OrderActionsBy(o => o.RelativePath);
                    });

                    var xmlPath = Path.Combine(BasePath, "Web.Core.xml");
                    c.IncludeXmlComments(xmlPath, true);

                    var xmlCommonPath = Path.Combine(BasePath, "Admin.Core.Common.xml");
                    c.IncludeXmlComments(xmlCommonPath, true);

                    var xmlModelPath = Path.Combine(BasePath, "Admin.Core.Model.xml");
                    c.IncludeXmlComments(xmlModelPath);

                    var xmlServicesPath = Path.Combine(BasePath, "Admin.Core.Service.xml");
                    c.IncludeXmlComments(xmlServicesPath);

                    //�������Token�İ�ť
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = "Value: Bearer {token}",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                    });

                    //���Jwt��֤����
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


            //#region Jwt�����֤

            var jwtConfig = _configHelper.Get<JwtConfig>("jwtconfig", _env.EnvironmentName);
            services.TryAddSingleton(jwtConfig);
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = nameof(ResponseAuthenticationHandler); //401
                options.DefaultForbidScheme = nameof(ResponseAuthenticationHandler); //403
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtConfig.Issuer,
                        ValidAudience = jwtConfig.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecurityKey)),
                        ClockSkew = TimeSpan.Zero
                    };
                })
                .AddScheme<AuthenticationSchemeOptions, ResponseAuthenticationHandler>(
                    nameof(ResponseAuthenticationHandler), o => { });

            //#endregion
        }


        public void ConfigureContainer(ContainerBuilder builder)
        {
            #region AutoFac IOC����

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

                builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
                builder.RegisterType<User>().As<IUser>().SingleInstance();
                builder.RegisterType<UserToken>().As<IUserToken>().InstancePerLifetimeScope();


                #region ����
                var cacheConfig = _configHelper.Get<CacheConfig>("cacheconfig", _env.EnvironmentName);
                if (cacheConfig.Type == CacheType.Redis)
                {
                    var csredis = new CSRedis.CSRedisClient(cacheConfig.Redis.ConnectionString);
                    RedisHelper.Initialization(csredis);

                    builder.RegisterType<RedisCache>().As<ICache>().SingleInstance();
                }
                else
                {
                    builder.RegisterType<MicrosoftMemoryCache.MemoryCache>().As<MicrosoftMemoryCache.IMemoryCache>().SingleInstance();
                    builder.RegisterType<MemoryCache>().As<ICache>().SingleInstance();
                }
                #endregion



                #region SingleInstance

                //�޽ӿ�ע�뵥��
                var assemblyCore = Assembly.Load("Web.Core");
                var assemblyCommon = Assembly.Load("Admin.Core.Common");
                builder.RegisterAssemblyTypes(assemblyCore, assemblyCommon)
                    .Where(t => t.GetCustomAttribute<SingleInstanceAttribute>() != null)
                    .SingleInstance();
                //�нӿ�ע�뵥��
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

            #region Swagger  Api文档

            if (!_env.IsDevelopment() && !_appConfig.Swagger) return;
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                typeof(ApiVersion).GetEnumNames().OrderByDescending(e => e).ToList().ForEach(version =>
                {
                    c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"Web.Core {version}");
                });
                c.RoutePrefix = ""; //ֱ�Ӹ�Ŀ¼����
                c.DocExpansion(DocExpansion.None); //�۵�Api
                //c.DefaultModelsExpandDepth(-1);//����ʾModels
            });

            #endregion


            //��֤
            app.UseAuthentication();

            //��Ȩ
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}