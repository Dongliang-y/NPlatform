#region << 版 本 注 释 >>

/*----------------------------------------------------------------
* 项目名称 ：NPlatform.IOC
* 类 描 述 ：
* 命名空间 ：NPlatform.IOC
* CLR 版本 ：4.0.30319.42000
* 作    者 ：DongliangYi
* 创建时间 ：2018-12-11 9:27:28
* 更新时间 ：2018-12-11 9:27:28
* 版 本 号 ：v1.0.0.0
//----------------------------------------------------------------*/

#endregion

namespace NPlatform.IOC
{
    using Autofac;
    using Autofac.Core;
    using Autofac.Core.Activators.Reflection;
    using Autofac.Extras.DynamicProxy;
    using AutoMapper;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using NPlatform.API.Controllers;
    using NPlatform.AutoMap;
    using NPlatform.Infrastructure.Config;
    using NPlatform.Repositories;
    using NPlatform.Repositories.IRepositories;
    using ServiceStack;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// ioc 管理类
    /// </summary>
    public static class IOCService
    {
        /// <summary>
        /// 异步锁
        /// </summary>
        private static readonly object SyncRoot = new object();

        /// <summary>
        /// IOC 容器对象
        /// </summary>
        private static ILifetimeScope container;


        /// <summary>
        /// IOC容器
        /// </summary>
        private static ILifetimeScope Container
        {
            get
            {
                lock (SyncRoot)
                {
                    if (container == null)
                    {
                        throw new NPlatformException("IOC 容器未初始化", "IOCManager.Container");
                    }

                    return container;
                }
            }
            set
            {
                container = value;
            }
        }

        /// <summary>
        /// 默认配置
        /// </summary>
        public static IRepositoryOptions DefaultOption { get; set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="IOCService"/> class from being created. 
        ///  IOC管理
        /// </summary>
        public static void CreateContainer(IRepositoryOptions rspOptions, IConfiguration config)
        {
            ContainerBuilder builder = new ContainerBuilder();
            Install(builder, rspOptions, config);
            container = builder.Build().BeginLifetimeScope();
        }

        /// <summary>
        /// 注入中间件
        /// </summary>
        /// <param name="rspOptions">
        /// The rsp Options.
        /// </param>
        /// <param name="builder">容器创建器</param>
        /// <param name="config">配置项</param>
        public static void Install(ContainerBuilder builder, IRepositoryOptions rspOptions, IConfiguration config)
        {
            builder.RegisterBuildCallback(scope =>
            {
                container = (ILifetimeScope)scope;
                var config = container.Resolve<IConfiguration>();
            });

            DefaultOption = rspOptions ?? new Repositories.RepositoryOptions();
            var currentDic = System.IO.Directory.GetCurrentDirectory();

            var serviceConfig = config.GetServiceConfig();
            var assemblyNames = serviceConfig.IOCAssemblys.Split(",");

            if (assemblyNames.Length == 0)
            {
                throw new NPlatformException("NPlatformConfig IOCAssemblys is null", "IOC Install");
            }

            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().AsImplementedInterfaces().PropertiesAutowired().InstancePerLifetimeScope();
            builder.RegisterType<PlatformHttpContext>().As<IPlatformHttpContext>().AsImplementedInterfaces().PropertiesAutowired().InstancePerLifetimeScope();
            builder.RegisterInstance<IRepositoryOptions>(DefaultOption).AsImplementedInterfaces().PropertiesAutowired().SingleInstance();




            var path = AppContext.BaseDirectory;
            Console.WriteLine(path);
            List<Assembly> assemblys = new List<Assembly>();

            foreach (var tmp in assemblyNames)
            {
                try
                {

                    var assPath = Path.Combine(path, tmp);
                    Assembly service = Assembly.LoadFrom(assPath);
                    assemblys.Add(service);
                }
                catch (FileNotFoundException ex)
                {
                    throw new NPlatformException(
                        $"没找到配置指定注入的{ex.FileName}，请确认dll是否已正确复制，名称是否与配置的相同。",
                        "IOCManager.Install.FileNotFoundException");
                }
            }

            //            InstancePerLifetimeScope：同一个Lifetime生成的对象是同一个实例
            //SingleInstance：单例模式，每次调用，都会使用同一个实例化的对象；每次都用同一个对象；
            //InstancePerDependency：默认模式，每次调用，都会重新实例化对象；每次请求都创建一个新的对象；

            var regBuilder = builder.RegisterAssemblyTypes(assemblys.ToArray()).Where(t =>
             (t.Name.EndsWith("Service")
             || t.Name.EndsWith("Application")
             || t.Name.EndsWith("Configs") && !t.Name.EndsWith("HostService")))
            .AsImplementedInterfaces()//
            .PropertiesAutowired() // 支持属性注入
            .InstancePerLifetimeScope()  // service 不能有状态，所以同一个生命周期内，一个实例就行。
            .EnableInterfaceInterceptors()
            .OnRegistered(e =>
             Console.WriteLine($"Service OnRegistered{e.ComponentRegistration.Activator.LimitType}")
            )
            .OnActivated(e =>
            Console.WriteLine($"OnActivated{e.Component.Activator.LimitType}"));

            // automapper
            builder.RegisterAssemblyTypes(assemblys.ToArray()).Where(t => typeof(Profile).IsAssignableFrom(t))
            .As<IProfile>()
            .AsImplementedInterfaces()//
            .SingleInstance() 
            .PropertiesAutowired()
            .OnRegistered(e =>
                Console.WriteLine($"Automapper OnRegistered{e.ComponentRegistration.Activator.LimitType}")
            )
            .OnActivated(e =>
            Console.WriteLine($"OnActivated{e.Component.Activator.LimitType}"));

            builder.RegisterType<MapperService>().As<IMapperService>()
            .AsImplementedInterfaces()//
            .SingleInstance()
            .PropertiesAutowired()
            .OnRegistered(e =>
                Console.WriteLine($"Automapper OnRegistered{e.ComponentRegistration.Activator.LimitType}")
            )
            .OnActivated(e =>
            Console.WriteLine($"OnActivated{e.Component.Activator.LimitType}"));

            builder.RegisterType<ResultProfile>()
            .As<IProfile>()
            .AsImplementedInterfaces()//
            .SingleInstance() 
            .PropertiesAutowired()
            .OnRegistered(e =>
                Console.WriteLine($"Automapper OnRegistered{e.ComponentRegistration.Activator.LimitType}")
            )
            .OnActivated(e =>
            Console.WriteLine($"OnActivated{e.Component.Activator.LimitType}"));

            


            // RegisterType
            // Repository
            builder.RegisterAssemblyTypes(assemblys.ToArray()).Where(t =>
             t.Name.EndsWith("Repository"))
            .AsImplementedInterfaces()
            .PropertiesAutowired()
            .InstancePerDependency()
            .OnRegistered(e =>
                Console.WriteLine($"Repository OnRegistered{e.ComponentRegistration.Activator.LimitType}")
            )
            .OnActivated(e =>
            Console.WriteLine($"OnActivated{e.Component.Activator.LimitType}"));


            //// Repository
            //builder.RegisterAssemblyTypes(assemblys.ToArray()).Where(t =>
            // t.Name.EndsWith("Repository"))
            //.InterceptedBy(typeof(LogInterceptor)).EnableInterfaceInterceptors(); //AOP 拦截器
            //.AsImplementedInterfaces()
            //.PropertiesAutowired()
            //.InstancePerDependency()
            //.OnRegistered(e =>
            //    Console.WriteLine($"Repository OnRegistered{e.ComponentRegistration.Activator.LimitType}")
            //)
            //.OnActivated(e =>
            //Console.WriteLine($"OnActivated{e.Component.Activator.LimitType}"));


            // Repository
            builder.RegisterAssemblyTypes(assemblys.ToArray()).Where(t =>
             t.Name.EndsWith("Controller"))
            .PropertiesAutowired()
            .InstancePerDependency()
            .OnRegistered(e =>
                Console.WriteLine($"Controller OnRegistered{e.ComponentRegistration.Activator.LimitType}")
            )
            .OnActivated(e =>
            Console.WriteLine($"OnActivated{e.Component.Activator.LimitType}"));
        }
        /// <summary>
        /// 获取automapper配置
        /// </summary>
        /// <returns>返回map配置类型</returns>
        public static IEnumerable<Profile> ResolveAutoMapper()
        {
            var result = container.Resolve<IEnumerable<IProfile>>();
            return result.Select(t => (Profile)t);
        }
    }
}