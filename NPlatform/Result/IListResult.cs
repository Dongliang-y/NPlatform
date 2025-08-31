/********************************************************************************

** auth： DongliangYi

** date： 2017/8/23 13:21:01

** desc： 尚未编写描述

** Ver.:  V1.0.0

*********************************************************************************/
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

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
        [JsonPropertyName("total")]
        long Total { get; set; }

        /// <summary>
        /// 数据行
        /// </summary>
        [DataMember]
        [JsonPropertyName("value")]
        new IEnumerable<T> Data { get; set; }

        /// <summary>
        /// 把结果转成List集合
        /// </summary>
        /// <returns></returns>
        IList<T> ToList();

        /// <summary>
        /// Total summary 
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public object[] summary { get; set; }
    }
}