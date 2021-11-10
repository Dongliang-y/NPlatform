/********************************************************************************

** auth： DongliangYi

** date： 2017/8/23 13:21:01

** desc： 尚未编写描述

** Ver.:  V1.0.0

*********************************************************************************/
using System;
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
    public class ListResult<T> :JsonResult, IListResult<T>
    {
        /// <summary>
        /// 数据列表内容对象
        /// </summary>
        public ListResult(IEnumerable<T> list, long total):base(list)
        {
            Total = total;
        }
        /// <summary>
        /// 数据列表内容对象
        /// </summary>
        public ListResult(IEnumerable<T> list):base(list)
        {
            Total = list.Count() ;
        }
        /// <summary>
        /// 数据列表内容对象
        /// </summary>
        public ListResult(IEnumerable<T> list, long total,HttpStatusCode httpCode):base(list)
        {
            Total = total;
            this.StatusCode = httpCode.ToInt();
        }
        /// <summary>
        /// ListResult
        /// </summary>
        /// <param name="list"></param>
        /// <param name="total"></param>
        /// <param name="httpCode"></param>
        /// <param name="serializerSettings"></param>
        public ListResult(IEnumerable<T> list, long total, HttpStatusCode httpCode, object serializerSettings) : base(list, serializerSettings)
        {
            Total = total;
            this.StatusCode = httpCode.ToInt();
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
        public string ServiceID { get; set; }

        /// <summary>
        ///  http heard contentType
        /// </summary>
        public new string ContentType { get; set; } = HttpContentType.APPLICATION_JSON;
        /// <summary>
        /// 状态码
        /// </summary>
        public new int? StatusCode { get; set; } = 200;

        /// <summary>
        /// 结果集合
        /// </summary>
        public new IEnumerable<T> Value { get; set; }

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