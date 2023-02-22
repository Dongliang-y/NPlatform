using AutoMapper;

namespace NPlatform.AutoMap
{
    public interface IMapperService
    {
        IMapper MyMapper { get; }

        object Map(object source, object destination, Type sourceType, Type destinationType);
        object Map(object source, Type sourceType, Type destinationType);
        TDestination Map<TDestination>(object source);
        TDestination Map<TDestination>(object source, Action<IMappingOperationOptions<object, TDestination>> opts);
        TDestination Map<TSource, TDestination>(TSource source);
        TDestination Map<TSource, TDestination>(TSource source, Action<IMappingOperationOptions<TSource, TDestination>> opts);
        TDestination Map<TSource, TDestination>(TSource source, TDestination destination);
        TDestination Map<TSource, TDestination>(TSource source, TDestination destination, Action<IMappingOperationOptions<TSource, TDestination>> opts);
    }
}