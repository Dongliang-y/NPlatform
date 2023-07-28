using AutoMapper;
using NPlatform.Result;

namespace NPlatform.AutoMap
{
    public class IListResultConverter<TS, TD> :
    ITypeConverter<IListResult<TS>, IListResult<TD>>
    {
        public IListResult<TD> Convert(IListResult<TS> source, IListResult<TD> destination, ResolutionContext context)
        {
            var values = context.Mapper.Map<IEnumerable<TD>>(source.Value);
            return new ListResult<TD>(values,source.Total);
        }
    }
}
