using AutoMapper;
using NPlatform.Result;
using System.Collections.Generic;

namespace NPlatform.AutoMap
{

    public class IEnumerableToListResultConverter<TS,TD> :
        ITypeConverter<IEnumerable<TS>, IListResult<TD>>
    {

        public IListResult<TD> Convert(IEnumerable<TS> source, IListResult<TD> destination, ResolutionContext context)
        {
            var values = context.Mapper.Map<IEnumerable<TD>>(source);
            return new ListResult<TD>(values);
        }
    }
}
