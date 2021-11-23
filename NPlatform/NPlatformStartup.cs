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
        /// 配置容器
        /// </summary>
        public static void Configure(ContainerBuilder builder, Repositories.RepositoryOptions options, IConfiguration config)
        {
            // 加载配置
            Config = config;
            Options = options;
            Console.WriteLine("ConfigureContainer");
            IOCService.Install(builder, Options, Config);
        }
    }
}