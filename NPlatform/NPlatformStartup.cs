#region << 版 本 注 释 >>

/*----------------------------------------------------------------
* 项目名称 ：ZJJWEPlatform.Domains
* 类 名 称 ：ZJJWEPlatformStartup
* 类 描 述 ：
* 命名空间 ：ZJJWEPlatform.Domains
* CLR 版本 ：4.0.30319.42000
* 作    者 ：DongliangYi
* 创建时间 ：2018-11-20 16:37:16
* 更新时间 ：2018-11-20 16:37:16
* 版 本 号 ：v1.0.0.0
//----------------------------------------------------------------*/

#endregion

namespace NPlatform
{
    using Autofac;
    using Consul;
    using IdentityModel.Client;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using NPlatform.DI;
    using NPlatform.Infrastructure.Config;
    // using Lincence.Verify;
    using NPlatform.Repositories;
    using System;

    /// <summary>
    /// 平台初始化对象. IOC容器加载、缓存初始化。
    /// </summary>
    public static class NPlatformStartup
    {
        /// <summary>
        /// 平台配置项
        /// </summary>

        public static IConfiguration Config { get; set; }


        /// <summary>
        /// 是否加载完成.
        /// </summary>
        public static bool AutoMapperInitialized { set; get; } = false;

        /// <summary>
        /// 仓储配置项
        /// </summary>
        public static Repositories.RepositoryOptions Options { get; set; }

        /// <summary>
        /// 配置容器
        /// </summary>
        public static void Configure(this ConfigureHostBuilder builder, IConfiguration config)
        {
            // 加载配置
            Config = config;
            var svcConfig = Config.GetServiceConfig();
            var options = new Repositories.RepositoryOptions()
            {
                DBProvider = DBProvider.MySqlClient,
                MainConection = svcConfig.MainConection, //使用简单的数据库集群
                MinorConnection = svcConfig.MinorConnection
            };
            //  Microsoft.AspNetCore.Mvc.ViewFeatures.Filters.ValidateAntiforgeryTokenAuthorizationFilter
            Options = options;
            Console.WriteLine("ConfigureContainer");

            IOCService.Install(builder, Options, Config);
        }

        public static void ConfigConsul(this Microsoft.Extensions.Hosting.IHostApplicationLifetime aft, IConfiguration config)
        {
            aft.ApplicationStopped.Register(() =>
            {
                var serviceConfig = config.GetServiceConfig();
                Console.WriteLine("DeregisterConsul");
                //请求注册的 Consul服务端 地址
                ConsulClient consulClient = new ConsulClient(p => { p.Address = new Uri(config.GetValue<string>("ConsulServer")); p.Datacenter = serviceConfig.DataCenterID; });
                consulClient.Agent.ServiceDeregister($"{serviceConfig.ServiceName}_{serviceConfig.DataCenterID}_{serviceConfig.ServiceID}");
            });

            aft.ApplicationStarted.Register(async () =>
            {
                var serviceConfig = config.GetServiceConfig();
                Console.WriteLine("RegisterConsul");
                //请求注册的 Consul服务端 地址
                ConsulClient consulClient = new ConsulClient(p => { p.Address = new Uri(config.GetValue<string>("ConsulServer")); p.Datacenter = serviceConfig.DataCenterID; });
                string urls = config.GetValue<string>("Urls");

                var urlArray = urls.Split(';', StringSplitOptions.RemoveEmptyEntries);
                if (urlArray.Length == 0)
                {
                    throw new Exception("必须在appsettings.json 配置 Urls项。");
                }
                var uri = new Uri(urlArray[0]);
                var httpCheck = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),//服务启动多久后注册
                    Interval = TimeSpan.FromSeconds(10),//间隔固定的时间访问一次，https://localhost:44308/api/Health
                    HTTP = $"{uri.Scheme}://{uri.Authority}/healthChecks",//健康检查地址 44308是visualstudio启动的端口
                    Timeout = TimeSpan.FromSeconds(5)
                };

                var registration = new AgentServiceRegistration()
                {
                    Checks = new[] { httpCheck },
                    ID = $"{serviceConfig.DataCenterID}_{serviceConfig.ServiceID}",
                    Name = serviceConfig.ServiceName,
                    Tags = new string[] { "1" },
                    Address = uri.Host,
                    Port = uri.Port,
                };
                try
                {
                    var rst = await consulClient.Agent.ServiceRegister(registration);//注册服务 
                                                                                     //consulClient.Agent.ServiceDeregister(registration.ID).Wait();//registration.ID是guid
                                                                                     //当服务停止时需要取消服务注册，不然，下次启动服务时，会再注册一个服务。
                    if (rst.StatusCode != System.Net.HttpStatusCode.OK)                                                //但是，如果该服务长期不启动，那consul会自动删除这个服务，大约2，3分钟就会删了 
                    {
                        Console.WriteLine(rst.StatusCode + rst.ToString());

                    }
                    else
                    {
                        Console.WriteLine($"{registration.ID}, name:{registration.Name}注册 consul成功");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    System.Diagnostics.Trace.WriteLine(ex.ToString());
                }
            });

        }
    }
}