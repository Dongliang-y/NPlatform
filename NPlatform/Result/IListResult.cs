/********************************************************************************

** auth： DongliangYi

** date： 2017/8/23 13:21:01

** desc： 尚未编写描述

** Ver.:  V1.0.0

*********************************************************************************/
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NPlatform.Result
{
    /// <summary>
    /// 数据列表内容对象
    /// </summary>
    public interface IListResult<T> : INPResult
    {

        /// <summary>
        /// 数据总数
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "total")]
        long Total { get; }

        /// <summary>
        /// 数据行
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "value")]
        new IEnumerable<T> Value { get; }

        /// <summary>
        /// 把结果转成List集合
        /// </summary>
        /// <returns></returns>
        IList<T> ToList();
    }
}