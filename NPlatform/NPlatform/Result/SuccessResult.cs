/********************************************************************************

** auth： DongliangYi

** date： 2017/8/23 13:21:01

** desc： 尚未编写描述

** Ver.:  V1.0.0

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using NPlatform;
using System.Net;
using System.Text.Json.Serialization;

namespace NPlatform.Result
{

    /// <summary>
    /// 操作结果
    /// </summary>
    public class SuccessResult<T> : INPResult
    {
        /// <summary>
        /// 需要返回的数据对象
        /// </summary>
        [DataMember]
        [JsonPropertyName("data")]
        public T Data { get; }
        /// <summary>
        /// 消息
        /// </summary>
        [DataMember]
        [JsonPropertyName("message")]
        public string Message { get;}

        /// <summary>
        /// HTTP status code
        /// </summary>
        [DataMember]
        [JsonPropertyName("httpcode")]
        public HttpStatusCode HttpCode { get; } = HttpStatusCode.OK;

        /// <summary>
        ///  返回结果的服务id
        /// </summary>
        [DataMember]
        [JsonPropertyName("serviceid")]
        public string ServiceID { get; set; }


        /// <summary>
        /// 操作结果
        /// </summary>
        public SuccessResult() { }


        /// <summary>
        /// 成功的结果内容
        /// </summary>
        /// <param name="message">消息</param>
        public SuccessResult(string message) 
        {
            this.Message = message;

        }
        /// <summary>
        /// 成功的结果内容
        /// </summary>
        /// <param name="data">消息</param>
        public SuccessResult(T data)
        {
            this.Data = data;
        }

        /// <summary>
        /// 成功的结果内容
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="httpCode">HttpStatusCode</param>
        public SuccessResult(string message,HttpStatusCode httpCode)
        {
            this.Message = message;
            this.HttpCode = httpCode;
        }

        /// <summary>
        /// 成功的结果内容
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="data">消息</param>
        public SuccessResult(string message, T data)
        {
            this.Message = message;
            this.Data = data;
        }

        /// <summary>
        /// 操作结果
        /// </summary>
        /// <param name="success">是否成功，默认true</param>
        /// <param name="message">消息</param>
        /// <param name="result">T 类型对象</param>
        /// <param name="httpCode"></param>
        public SuccessResult( string message, T result, HttpStatusCode httpCode)
        {
            this.HttpCode = httpCode;
            this.Message = message;
            this.Data = result;
        }

    }
}