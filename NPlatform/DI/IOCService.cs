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


using Autofac;
using Autofac.Extras.DynamicProxy;
using AutoMapper;
using MediatR.Extensions.Autofac.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Hosting;
using NPlatform.AutoMap;
using NPlatform.Domains.Service;
using NPlatform.Exceptions;
using NPlatform.Filters;
using NPlatform.Infrastructure.Config;
using NPlatform.Infrastructure.Redis;
using NPlatform.Repositories;
using NPlatform.Repositories.IRepositories;
using NPlatform.Result;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Asn1.X509.Qualified;
using System.Reflection;

namespace NPlatform.DI
{

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
        public static ILifetimeScope Container
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
        /// 注入中间件
        /// </summary>
        /// <param name="rspOptions">
        /// The rsp Options.
        /// </param>
        /// <param name="builder">容器创建器</param>
        /// <param name="config">配置项</param>
        public static void Install(ConfigureHostBuilder builder, IRepositoryOptions rspOptions, IConfiguration config)
        {
            builder.ConfigureContainer<ContainerBuilder>(builder =>
            {
                builder.RegisterBuildCallback(scope =>
                {
                    container = scope;
                    var config = container.Resolve<IConfiguration>();
                });

                DefaultOption = rspOptions ?? new RepositoryOptions();
                var currentDic = Directory.GetCurrentDirectory();

                var serviceConfig = config.GetServiceConfig();
                var assemblyNames = serviceConfig.IOCAssemblys.Split(",", StringSplitOptions.RemoveEmptyEntries);

                if (assemblyNames.Length == 0)
                {
                    throw new NPlatformException("NPlatformConfig IOCAssemblys is null", "IOC Install");
                }
                //        public IRedisService _RedisService { get; set; }

                builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().AsImplementedInterfaces().PropertiesAutowired(new AutowiredSelector()).InstancePerLifetimeScope().OnRegistered(e =>
                 Console.WriteLine($"OnRegistered {e.ComponentRegistration.Activator.LimitType.FullName}")
                ); ;
                builder.RegisterType<PlatformHttpContext>().As<IPlatformHttpContext>().AsImplementedInterfaces().PropertiesAutowired(new AutowiredSelector()).InstancePerLifetimeScope().OnRegistered(e =>
                 Console.WriteLine($"OnRegistered {e.ComponentRegistration.Activator.LimitType.FullName}")
                ); ;
                builder.RegisterInstance(DefaultOption).As<IRepositoryOptions>().AsImplementedInterfaces().PropertiesAutowired(new AutowiredSelector()).SingleInstance().OnRegistered(e =>
                 Console.WriteLine($"OnRegistered {e.ComponentRegistration.Activator.LimitType.FullName}")
                ); ;
                builder.RegisterType<RedisService>().As<IRedisService>().AsImplementedInterfaces().PropertiesAutowired(new AutowiredSelector()).InstancePerLifetimeScope().OnRegistered(e =>
                 Console.WriteLine($"OnRegistered {e.ComponentRegistration.Activator.LimitType.FullName}")
                );

                builder.RegisterType<ValidateAntiforgeryAuthorizationFilter>().As<IAntiforgeryPolicy>().AsImplementedInterfaces().PropertiesAutowired(new AutowiredSelector()).InstancePerLifetimeScope().OnRegistered(e =>
                 Console.WriteLine($"Service OnRegistered ValidateAntiforgeryAuthorizationFilter")
                )
                .OnActivated(e =>
                Console.WriteLine($"OnActivated ValidateAntiforgeryAuthorizationFilter"));

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
                assemblys.Add(Assembly.GetCallingAssembly());
                assemblys.Add(Assembly.GetEntryAssembly());
                //Assembly.GetExecutingAssembly()
                //            InstancePerLifetimeScope：同一个Lifetime生成的对象是同一个实例
                //SingleInstance：单例模式，每次调用，都会使用同一个实例化的对象；每次都用同一个对象；
                //InstancePerDependency：默认模式，每次调用，都会重新实例化对象；每次请求都创建一个新的对象；

                var regBuilder = builder.RegisterAssemblyTypes(assemblys.ToArray()).Where(t =>
                t.IsSubclassOf(typeof(BaseService))
                // t.Name.EndsWith("Service")
                 || //t.Name.EndsWith("Applications")
                  t.IsSubclassOf(typeof(Applications.ApplicationService))
                 || t.Name.EndsWith("Configs") && !t.Name.EndsWith("HostService")
                 )
                .AsImplementedInterfaces()//
                .PropertiesAutowired(new AutowiredSelector()) // 支持属性注入 LoginApplication
                .InstancePerLifetimeScope()  // service 不能有状态，所以同一个生命周期内，一个实例就行。
                .EnableInterfaceInterceptors()
                .OnRegistered(e =>
                 Console.WriteLine($"Service OnRegistered{e.ComponentRegistration.Activator.LimitType.FullName}")
                )
                .OnActivated(e =>
                Console.WriteLine($"OnActivated{e.Component.Activator.LimitType.FullName}"));



                // automapper
                builder.RegisterAssemblyTypes(assemblys.ToArray()).Where(t => typeof(Profile).IsAssignableFrom(t))
                .As<IProfile>()
                .AsImplementedInterfaces()//
                .SingleInstance()
                .PropertiesAutowired(new AutowiredSelector())
                .OnRegistered(e =>
                    Console.WriteLine($"Automapper OnRegistered{e.ComponentRegistration.Activator.LimitType.FullName}")
                )
                .OnActivated(e =>
                Console.WriteLine($"OnActivated{e.Component.Activator.LimitType.FullName}"));

                builder.RegisterType<MapperService>().As<IMapperService>()
                .AsImplementedInterfaces()//
                .SingleInstance()
                .PropertiesAutowired(new AutowiredSelector())
                .OnRegistered(e =>
                    Console.WriteLine($"Automapper OnRegistered{e.ComponentRegistration.Activator.LimitType.FullName}")
                )
                .OnActivated(e =>
                Console.WriteLine($"OnActivated{e.Component.Activator.LimitType.FullName}"));

                builder.RegisterType<ResultProfile>()
                .As<IProfile>()
                .AsImplementedInterfaces()//
                .SingleInstance()
                .PropertiesAutowired(new AutowiredSelector())
                .OnRegistered(e =>
                    Console.WriteLine($"Automapper OnRegistered{e.ComponentRegistration.Activator.LimitType.FullName}")
                )
                .OnActivated(e =>
                Console.WriteLine($"OnActivated{e.Component.Activator.LimitType.FullName}"));


                builder.RegisterAssemblyTypes(assemblys.ToArray()).Where(t =>
                 t.Name.EndsWith("Result"))
                .AsImplementedInterfaces()
                .PropertiesAutowired(new AutowiredSelector())
                .InstancePerLifetimeScope()
                .OnRegistered(e =>
                    Console.WriteLine($"Result OnRegistered{e.ComponentRegistration.Activator.LimitType.FullName}")
                )
                .OnActivated(e =>
                Console.WriteLine($"OnActivated{e.Component.Activator.LimitType.FullName}"));

                //builder.RegisterType(typeof(NPActionResultExecutor))
                //.As(typeof(IActionResultExecutor<INPResult>))
                //.AsImplementedInterfaces()//
                //.SingleInstance()
                //.PropertiesAutowired()
                //.OnRegistered(e =>
                //    Console.WriteLine($"NPActionResultExecutor OnRegistered{e.ComponentRegistration.Activator.LimitType}")
                //)
                //.OnActivated(e =>
                //Console.WriteLine($"OnActivated{e.Component.Activator.LimitType}"));

                // RegisterType
                // Repository
                builder.RegisterAssemblyTypes(assemblys.ToArray()).Where(t =>
                 t.Name.EndsWith("Repository"))
                .AsImplementedInterfaces()
                .PropertiesAutowired(new AutowiredSelector())
                .InstancePerLifetimeScope()
                .OnRegistered(e =>
                    Console.WriteLine($"Repository OnRegistered{e.ComponentRegistration.Activator.LimitType.FullName}")
                )
                .OnActivated(e =>
                Console.WriteLine($"OnActivated{e.Component.Activator.LimitType.FullName}"));

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
                builder.RegisterAssemblyTypes(assemblys.ToArray())
                .Where(t =>t.Name.EndsWith("Controller"))
                .PropertiesAutowired(new AutowiredSelector())
                .InstancePerDependency()
                .OnRegistered(e =>
                    Console.WriteLine($"Controller OnRegistered{e.ComponentRegistration.Activator.LimitType.FullName}")
                )
                .OnActivated(e =>
                Console.WriteLine($"OnActivated{e.Component.Activator.LimitType.FullName}"));
                
                // Command
                builder.RegisterAssemblyTypes(assemblys.ToArray()).Where(t => typeof(ICommand).IsAssignableFrom(t))
                .As<MediatR.IRequest<INPResult>>()
                .AsImplementedInterfaces()//
                .SingleInstance()
                .PropertiesAutowired(new AutowiredSelector())
                .OnRegistered(e =>
                    Console.WriteLine($"Automapper OnRegistered{e.ComponentRegistration.Activator.LimitType.FullName}")
                )
                .OnActivated(e =>
                Console.WriteLine($"OnActivated{e.Component.Activator.LimitType.FullName}"));


            });
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

        /// <summary>
        /// 获取automapper配置
        /// </summary>
        /// <returns>返回map配置类型</returns>
        public static IEnumerable<T> ResolveDBContext<T>()
        {
            var result = container.Resolve<IEnumerable<T>>();
            return result.Select(t => (T)t);
        }

        ///// <summary>
        ///// 获取automapper配置
        ///// </summary>
        ///// <returns>返回map配置类型</returns>
        //public static IActionResultExecutor<INPResult> ResolveNPActionResultExecutor()
        //{
        //    var result = container.Resolve<IActionResultExecutor<INPResult>>();
        //    return result;
        //}
    }
}