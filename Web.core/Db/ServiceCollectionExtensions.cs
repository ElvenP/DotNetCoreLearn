using System;
using System.Threading.Tasks;
using Admin.Core.Common.Auth;
using Admin.Core.Common.BaseModel;
using Admin.Core.Common.Configs;
using Admin.Core.Common.Helpers;
using FreeSql;
using FreeSql.Aop;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Web.core.Db
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     添加数据库
        /// </summary>
        /// <param name="services"></param>
        /// <param name="env"></param>
        /// <param name="appConfig"></param>
        public static async void AddDb(this IServiceCollection services, IHostEnvironment env, AppConfig appConfig)
        {
            var dbConfig = new ConfigHelper().Get<DbConfig>("dbconfig", env.EnvironmentName);

            //创建数据库
            if (dbConfig.CreateDb) await DbHelper.CreateDataBase(dbConfig);

            #region FreeSql

            var freeSqlBuilder = new FreeSqlBuilder()
                .UseConnectionString(dbConfig.Type, dbConfig.ConnectionString)
                .UseAutoSyncStructure(dbConfig.SyncStructure)
                .UseLazyLoading(false)
                .UseNoneCommandParameter(true);

            #region 监听所有命令

            if (dbConfig.MonitorCommand)
                freeSqlBuilder.UseMonitorCommand(cmd => { }, (cmd, traceLog) =>
                {
                    //Console.WriteLine($"{cmd.CommandText}\n{traceLog}\r\n");
                    Console.WriteLine($"{cmd.CommandText}\r\n");
                });

            #endregion

            var fsql = freeSqlBuilder.Build();
            //fsql.GlobalFilter.Apply<IEntitySoftDelete>("SoftDelete", a => a.IsDeleted == false);
            services.AddFreeRepository(filter =>
                filter.Apply<IEntitySoftDelete>("SoftDelete", a => a.IsDeleted == false));
            services.AddScoped<UnitOfWorkManager>();
            services.AddSingleton(fsql);

            #region 初始化数据库

            //同步结构
            if (dbConfig.SyncStructure) DbHelper.SyncStructure(fsql, dbConfig: dbConfig);

            //同步数据
            if (dbConfig.SyncData)
            {
                //await DbHelper.SyncData(fsql, dbConfig);
            }

            #endregion

            //生成数据包
            if (dbConfig.GenerateData && !dbConfig.CreateDb && !dbConfig.SyncData)
                await DbHelper.GenerateSimpleJsonData(fsql);

            #region 监听Curd操作

            if (dbConfig.Curd)
                fsql.Aop.CurdBefore += (s, e) =>
                {
                    Parallel.For(0, 1, body => { Console.WriteLine($"{e.Sql}\r\n"); });
                };

            #endregion

            #region 审计数据

            //计算服务器时间
            //var serverTime = fsql.Select<T>().Limit(1).First(a => DateTime.local);
            //var timeOffset = DateTime.UtcNow.Subtract(serverTime);
            var user = services.BuildServiceProvider().GetService<IUser>();
            fsql.Aop.AuditValue += (s, e) =>
            {
                if (user == null || user.Id <= 0) return;

                e.Value = e.AuditValueType switch
                {
                    AuditValueType.Insert => e.Property.Name switch
                    {
                        "CreatedUserId" => user.Id,
                        "CreatedUserName" => user.Name,
                        _ => e.Value
                    },
                    AuditValueType.Update => e.Property.Name switch
                    {
                        "ModifiedUserId" => user.Id,
                        "ModifiedUserName" => user.Name,
                        _ => e.Value
                    },
                    _ => e.Value
                };
            };

            #endregion

            #endregion

            Console.WriteLine($"{appConfig.Urls}\r\n");
        }
    }
}