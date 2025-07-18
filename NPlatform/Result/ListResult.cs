﻿/********************************************************************************

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
    [DataContract]
    public class ListResult<T> : IListResult<T>
    {

        /// <summary>
        /// 数据总数
        /// </summary>
        [DataMember]
        public long Total { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        ///  返回结果的服务id
        /// </summary>
        [DataMember]
        public string ServiceID { get; set; }

        /// <summary>
        ///  http heard contentType
        /// </summary>
        [DataMember]
        public string? ContentType { get; set; } = HttpContentType.APPLICATION_JSON;
        /// <summary>
        /// 状态码
        /// </summary>
        [DataMember]
        public int? StatusCode { get; set; } = 200;

        [DataMember]
        public IEnumerable<T> Data { get; set; }

        //
        // 摘要:
        //     Total summary calculation results.
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public object[] summary { get; set; }

        #region 不序列化返回的属性
        [JsonIgnore]
        [System.Xml.Serialization.XmlIgnore]
        object INPResult.Data { get { return this.Data; } set { this.Data = value as IEnumerable<T>; } }

        [JsonIgnore]
        [System.Xml.Serialization.XmlIgnore]
        public object SerializerSettings { get; set; }=new JsonSerializerOptions
        {
               DefaultIgnoreCondition= JsonIgnoreCondition.Never
        };
        #endregion
        /// <summary>
        /// 数据列表内容对象
        /// </summary>
        public ListResult()
        {
        }
        /// <summary>
        /// 数据列表内容对象
        /// </summary>
        public ListResult(IEnumerable<T> list, long total)
        {
            Total = total;
            this.Data = list;
        }
        /// <summary>
        /// 数据列表内容对象
        /// </summary>
        public ListResult(IEnumerable<T> list)
        {
            Total = list.Count();
            this.Data = list;
        }
        /// <summary>
        /// 数据列表内容对象
        /// </summary>
        public ListResult(IEnumerable<T> list, long total, object[] sums, int? httpCode)
        {
            Total = total;
            this.StatusCode = httpCode;
            this.Data = list;
            summary = sums;
        }

        /// <summary>
        /// 返回结果的集合
        /// </summary>
        /// <returns>结果集合</returns>
        public IList<T> ToList()
        {
            return this.Data.ToList();
        }

        /// <inheritdoc />
        public async Task ExecuteResultAsync(ActionContext context)
        {
            await new JsonResult(this, SerializerSettings).ExecuteResultAsync(context);
        }
    }
}