using AutoMapper;
using NPlatform.Result;
using System.Collections.Generic;

namespace NPlatform.AutoMap
{
    /// <summary>
    /// 配置 result 使用automaper转换的规则，方便类似 INResult~Entity 到 List~Dto的直接转换
    /// </summary>
    public class ResultProfile : Profile,IProfile
    {
        /// <summary>
        /// 配置可以互转的类
        /// </summary>
        public ResultProfile()
        {
            CreateMap(typeof(IEnumerable<>), typeof(IListResult<>)).ConvertUsing(typeof(IEnumerableToListResultConverter<,>));
            CreateMap(typeof(IListResult<>), typeof(IListResult<>)).ConvertUsing(typeof(IListResultConverter<,>));
            CreateMap(typeof(INPResult), typeof(INPResult));
        }
    }
}
