using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.Helpers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPlatform
{
    /// <summary>
    /// DataSourceLoadOptions
    /// </summary>
    [ModelBinder(BinderType = typeof(DataSourceLoadOptionsBinder))]
    public class DataSourceLoadOptions : DataSourceLoadOptionsBase
    {
        public void And(DataSourceLoadOptionsBase dataSourceLoadOptions, List<object> customFilters)
        {
            if (customFilters == null || customFilters.Count == 0)
            {
                return; // 无额外筛选条件
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
        public void OR(DataSourceLoadOptionsBase dataSourceLoadOptions, List<object> customFilters)
        {
            if (customFilters == null || customFilters.Count == 0)
            {
                return; // 无额外筛选条件
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

        /// <summary>
        /// 检查 DataSourceLoadOptionsBase 的 Filter 属性是否包含名为 "IsDelete" 的条件。
        /// 假设 Filter 属性已经通过 JsonConverter 反序列化为 IList。
        /// </summary>
        public bool HasIsDelete(DataSourceLoadOptionsBase loadOptions)
        {
            if (loadOptions == null || loadOptions.Filter == null)
            {
                return false;
            }

            // Filter 属性已经是 IList，无需再次反序列化 JSON 字符串
            // 而是直接递归检查这个 IList 结构
            return HasIsDeleteFilterRecursive(loadOptions.Filter);
        }

        /// <summary>
        /// 递归检查反序列化后的过滤条件列表是否包含 "IsDelete"。
        /// </summary>
        private bool HasIsDeleteFilterRecursive(object filter)
        {
            if (filter == null)
            {
                return false;
            }

            // 检查是否是 IList 类型
            if (filter is IList list)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];

                    // 检查当前项是否是另一个列表（嵌套过滤或三元组过滤）
                    if (item is IList subList)
                    {
                        // 递归检查嵌套的列表
                        if (HasIsDeleteFilterRecursive(subList))
                        {
                            return true;
                        }
                    }
                    // 检查是否是字符串（逻辑运算符或字段名）
                    else if (item is string strItem)
                    {
                        // 如果这是逻辑运算符，则继续迭代当前列表
                        if (strItem.Equals("and", StringComparison.OrdinalIgnoreCase) ||
                            strItem.Equals("or", StringComparison.OrdinalIgnoreCase) ||
                            strItem.Equals("!", StringComparison.OrdinalIgnoreCase))
                        {
                            // 继续处理列表中的下一个元素
                        }
                        else
                        {
                            // 这是一个潜在的字段名、运算符或值。
                            // 对于过滤条件，我们关心的是一个三元组 ['FieldName', 'Operator', 'Value']
                            // 所以，如果当前列表的长度是 3，并且当前项是列表的第一个元素 (索引 0)，
                            // 并且这个元素是 "IsDelete"，那么我们就找到了。

                            // 需要将 'item' 转换为其所属的 'list' 的第一个元素进行检查
                            // 但是，由于我们是迭代 list，所以 item 就是 list[i]
                            // 我们需要查看它所在的 'list' 是否是三元组。
                            // 所以我们检查 item 是否是一个 string，并且它所在列表的第一个元素（如果 `item` 就是那个第一个元素的话）是 "IsDelete"。

                            // 更精确的检查是：如果 `list` 是一个长度为 3 的列表，并且 `list[0]` 是 "IsDelete"
                            // 在当前循环中，`item` 就是 `list[i]`
                            // 所以我们应该检查 `list` 是否长度是 3，且 `list[0]` 是 "IsDelete"
                            if (list.Count == 3 && i == 0 && strItem.Equals("IsDelete", StringComparison.OrdinalIgnoreCase))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            // 如果 Filter 本身不是列表（例如，它是一个空集合，或者是一个单独的过滤条件（不太可能，因为通常是数组包裹））。
            // 根据 DevExtreme 的设计，Filter 大部分情况会是一个 JSON 数组。
            else
            {
                // 如果 filter 是一个 string，并且它就是 "IsDelete"，那我们也可以认为它匹配了。
                // 但鉴于 Filter 是IList，这种情况可能不会直接发生，除非IList里只有一个 string 元素。
                if (filter is string strFilter && strFilter.Equals("IsDelete", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class DataSourceLoadOptionsBinder : IModelBinder
    {

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var loadOptions = new DataSourceLoadOptions();
            DataSourceLoadOptionsParser.Parse(loadOptions, key => bindingContext.ValueProvider.GetValue(key).FirstOrDefault());
            bindingContext.Result = ModelBindingResult.Success(loadOptions);
            return Task.CompletedTask;
        }
    }
}
