using AutoMapper;
using NPlatform.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WXWorkFinanceApproveApp.Services
{
    /// <summary>
    /// Automapper 配置，注意，需要单例模式注入,封装下~
    /// </summary>
    public class ObjectMapper
    {
        public IMapper MyMapper { get; private set; }
        /// <summary>
        /// 初始化配置
        /// </summary>
        public  ObjectMapper()
        {
            var config = new MapperConfiguration(cfg =>
              {
                  cfg.AddProfiles(IOCService.ResolveAutoMapper());
              });
            MyMapper = new Mapper(config);
        }

        //
        // 摘要:
        //     Execute a mapping from the source object to a new destination object. The source
        //     type is inferred from the source object.
        //
        // 参数:
        //   source:
        //     Source object to map from
        //
        // 类型参数:
        //   TDestination:
        //     Destination type to create
        //
        // 返回结果:
        //     Mapped destination object
        public TDestination Map<TDestination>(object source)
        {
            return MyMapper.Map<TDestination>(source);
        }
        //
        // 摘要:
        //     Execute a mapping from the source object to a new destination object.
        //
        // 参数:
        //   source:
        //     Source object to map from
        //
        // 类型参数:
        //   TSource:
        //     Source type to use, regardless of the runtime type
        //
        //   TDestination:
        //     Destination type to create
        //
        // 返回结果:
        //     Mapped destination object
        public TDestination Map<TSource, TDestination>(TSource source)
        {
            return MyMapper.Map<TSource, TDestination>(source);
        }
        //
        // 摘要:
        //     Execute a mapping from the source object to the existing destination object.
        //
        // 参数:
        //   source:
        //     Source object to map from
        //
        //   destination:
        //     Destination object to map into
        //
        // 类型参数:
        //   TSource:
        //     Source type to use
        //
        //   TDestination:
        //     Destination type
        //
        // 返回结果:
        //     The mapped destination object, same instance as the destination object
        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return MyMapper.Map<TSource, TDestination>(source, destination);
        }
        //
        // 摘要:
        //     Execute a mapping from the source object to a new destination object with explicit
        //     System.Type objects
        //
        // 参数:
        //   source:
        //     Source object to map from
        //
        //   sourceType:
        //     Source type to use
        //
        //   destinationType:
        //     Destination type to create
        //
        // 返回结果:
        //     Mapped destination object
       public object Map(object source, Type sourceType, Type destinationType)
        {
            return MyMapper.Map(source, sourceType, destinationType);
        }
        //
        // 摘要:
        //     Execute a mapping from the source object to existing destination object with
        //     explicit System.Type objects
        //
        // 参数:
        //   source:
        //     Source object to map from
        //
        //   destination:
        //     Destination object to map into
        //
        //   sourceType:
        //     Source type to use
        //
        //   destinationType:
        //     Destination type to use
        //
        // 返回结果:
        //     Mapped destination object, same instance as the destination object
        public object Map(object source, object destination, Type sourceType, Type destinationType)
        {
            return MyMapper.Map(source, destination,sourceType, destinationType);
        }

        //
        // 摘要:
        //     Execute a mapping from the source object to a new destination object with supplied
        //     mapping options.
        //
        // 参数:
        //   source:
        //     Source object to map from
        //
        //   opts:
        //     Mapping options
        //
        // 类型参数:
        //   TDestination:
        //     Destination type to create
        //
        // 返回结果:
        //     Mapped destination object
        public TDestination Map<TDestination>(object source, Action<IMappingOperationOptions<object, TDestination>> opts)
        {
            return MyMapper.Map(source, opts);
        }
        //
        // 摘要:
        //     Execute a mapping from the source object to a new destination object with supplied
        //     mapping options.
        //
        // 参数:
        //   source:
        //     Source object to map from
        //
        //   opts:
        //     Mapping options
        //
        // 类型参数:
        //   TSource:
        //     Source type to use
        //
        //   TDestination:
        //     Destination type to create
        //
        // 返回结果:
        //     Mapped destination object
        public TDestination Map<TSource, TDestination>(TSource source, Action<IMappingOperationOptions<TSource, TDestination>> opts)
        {
            return MyMapper.Map(source, opts);
        }
        //
        // 摘要:
        //     Execute a mapping from the source object to the existing destination object with
        //     supplied mapping options.
        //
        // 参数:
        //   source:
        //     Source object to map from
        //
        //   destination:
        //     Destination object to map into
        //
        //   opts:
        //     Mapping options
        //
        // 类型参数:
        //   TSource:
        //     Source type to use
        //
        //   TDestination:
        //     Destination type
        //
        // 返回结果:
        //     The mapped destination object, same instance as the destination object
        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination, Action<IMappingOperationOptions<TSource, TDestination>> opts)
        {
            return MyMapper.Map(source, destination,opts);
        }
    }
}
