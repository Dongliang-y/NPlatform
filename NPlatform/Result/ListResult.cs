/********************************************************************************

** auth： DongliangYi

** date： 2017/8/23 13:21:01

** desc： 尚未编写描述

** Ver.:  V1.0.0

*********************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using NPlatform;
using NPlatform.Infrastructure;

namespace NPlatform.Result
{
    /// <summary>
    /// 数据列表内容对象
    /// </summary>
    [DataContract]
    public class ListResult<T> : IListResult<T>
    {
        /// <summary>
        /// 数据列表内容对象
        /// </summary>
        public ListResult(IEnumerable<T> list, long total)
        {
            Total = total;
            this.Value = list;
        }
        /// <summary>
        /// 数据列表内容对象
        /// </summary>
        public ListResult(IEnumerable<T> list) 
        {
            Total = list.Count();
            this.Value = list;
        }
        /// <summary>
        /// 数据列表内容对象
        /// </summary>
        public ListResult(IEnumerable<T> list, long total, int? httpCode)
        {
            Total = total;
            this.StatusCode = httpCode;
            this.Value = list;
        }
        /// <summary>
        /// 数据总数
        /// </summary>
        [DataMember]
        [JsonPropertyName("total")]
        public long Total { get; }

        /// <summary>
        /// 消息
        /// </summary>
        [DataMember]
        [JsonPropertyName("message")]
        public string Message { get; }

        /// <summary>
        ///  返回结果的服务id
        /// </summary>
        [DataMember]
        [JsonPropertyName("serviceid")]
        [JsonIgnore]
        [System.Xml.Serialization.XmlIgnore]
        public string ServiceID { get; set; }

        /// <summary>
        ///  http heard contentType
        /// </summary>
        public string ContentType { get; set; } = HttpContentType.APPLICATION_JSON;
        /// <summary>
        /// 状态码
        /// </summary>
        public int? StatusCode { get; set; } = 200;

        public IEnumerable<T> Value { get; set; }

        object INPResult.Value { get { return this.Value; } set { this.Value = value as IEnumerable<T>; } }


        /// <summary>
        /// 返回结果的集合
        /// </summary>
        /// <returns>结果集合</returns>
        public IList<T> ToList()
        {
            return this.Value.ToList();
        }
    }
}