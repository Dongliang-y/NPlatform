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
    using AutoMapper;
   // using Lincence.Verify;
    using System.IO;
    using NPlatform.Infrastructure;
    using NPlatform.Filters;
    using NPlatform.IOC;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Configuration;
    using NPlatform.Infrastructure.Config;
    using Autofac;
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
        /// 初始化平台配置
        /// </summary>
        /// <param name="config">系统配置</param>
        /// <param name="options">仓储的配置项</param>
        public static void AddNPlatformConfig(Repositories.RepositoryOptions options, IConfiguration config)
        {
            // 加载配置
            Config = config;
            Options = options;
            AutoMapperInit();

            
        }

        /// <summary>
        /// 配置容器
        /// </summary>
        public static void ConfigureContainer(ContainerBuilder builder,
                                              Repositories.RepositoryOptions options,
                                              IConfiguration config)
        {
            Console.WriteLine("ConfigureContainer");
            IOCService.Install(builder, options, config);
        }
        /// <summary>
        /// AutoMapper初始化
        /// </summary>
        private static void AutoMapperInit()
        {
            Mapper.Initialize(cfg =>
                {
                    AutoMapperInitialized = false;
                    var maps = IOCService.ResolveAutoMapper();
                    foreach (var map in maps)
                    {
                        map.Config(cfg);
                    }
                    AutoMapperInitialized = true;
                });
        }
    }
}