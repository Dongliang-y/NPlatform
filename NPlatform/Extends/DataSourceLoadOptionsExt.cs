using DevExtreme.AspNet.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BZPT
{
    public static class DataSourceLoadOptionsExt
    {
        public static void And(this DataSourceLoadOptionsBase dataSourceLoadOptions, List<object> customFilters)
        {
            if (customFilters == null || customFilters.Count == 0)
            {
                return ; // 无额外筛选条件
            }

            if (dataSourceLoadOptions.Filter == null || dataSourceLoadOptions.Filter.Count == 0)
            {
                dataSourceLoadOptions.Filter = customFilters;
            }
            else
            {

                dataSourceLoadOptions.Filter = new List<object>() { dataSourceLoadOptions.Filter, "and", customFilters };
            }
        }
        public static void OR(this DataSourceLoadOptionsBase dataSourceLoadOptions, List<object> customFilters)
        {
            if (customFilters == null || customFilters.Count == 0)
            {
                return ; // 无额外筛选条件
            }

            if (dataSourceLoadOptions.Filter == null || dataSourceLoadOptions.Filter.Count == 0)
            {
                dataSourceLoadOptions.Filter = customFilters;
            }
            else
            {

                dataSourceLoadOptions.Filter = new List<object>() { dataSourceLoadOptions.Filter, "or", customFilters };
            }
        }
    }
}
