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
using NPlatform;

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
            Data = list;
        }
        /// <summary>
        /// 数据列表内容对象
        /// </summary>
        public ListResult(IEnumerable<T> list)
        {
            Total = list.Count() ;
            Data = list;
        }
        /// <summary>
        /// 数据列表内容对象
        /// </summary>
        public ListResult(IEnumerable<T> list, long total,HttpStatusCode httpCode)
        {
            Total = total;
            Data = list;
            this.HttpCode = httpCode;
        }
        /// <summary>
        /// 数据列表内容对象
        /// </summary>
        public ListResult(IEnumerable<T> list, HttpStatusCode httpCode)
        {
            Total = list.Count();
            Data = list;
            this.HttpCode = httpCode;
        }

        /// <summary>
        /// 数据总数
        /// </summary>
        [DataMember]
        [JsonPropertyName("total")]
        public long Total { get; }

        /// <summary>
        /// 数据行
        /// </summary>
        /// <summary>
        /// OrgCode
        /// </summary>
        [DataMember]
        [JsonPropertyName("data")]
        public IEnumerable<T> Data { get; }
        /// <summary>
        /// 消息
        /// </summary>
        [DataMember]
        [JsonPropertyName("message")]
        public string Message { get;}


        /// <summary>
        /// HTTP状态码
        /// </summary>
        [DataMember]
        [JsonPropertyName("httpcode")]
        public HttpStatusCode HttpCode { get;} = HttpStatusCode.OK;

        /// <summary>
        ///  返回结果的服务id
        /// </summary>
        [DataMember]
        [JsonPropertyName("serviceid")]
        public string ServiceID { get; set; }

        /// <summary>
        /// 返回结果的集合
        /// </summary>
        /// <returns>结果集合</returns>
        public IList<T> ToList()
        {
            return this.Data.ToList();
        }
    }
}