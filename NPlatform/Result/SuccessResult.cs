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
using Microsoft.AspNetCore.Mvc;
using NPlatform.Infrastructure;

namespace NPlatform.Result
{

    /// <summary>
    /// 操作结果
    /// </summary>
    public class SuccessResult<T> : ActionResult, INPResult
    {
        /// <summary>
        /// 消息
        /// </summary>
        [DataMember]
        [JsonPropertyName("message")]
        public string Message { get;}

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
        public new string ContentType { get; set; } = HttpContentType.APPLICATION_JSON;
        /// <summary>
        /// 状态码
        /// </summary>
        public new int? StatusCode { get; set; }=200;

        public object Value { get; set; }

        /// <summary>
        /// 成功的结果内容
        /// </summary>
        public SuccessResult()
        {
        }
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
            this.Value = data;
        }

        /// <summary>
        /// 成功的结果内容
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="data">消息</param>
        public SuccessResult(string message, T data)
        {
            this.Message = message;
        }

        /// <summary>
        /// 操作结果
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="data">T 类型对象</param>
        /// <param name="httpCode"></param>
        /// <param name="serializerSettings">序列化配置</param>
        public SuccessResult( string message, T data, HttpStatusCode httpCode,object serializerSettings)
        {
            this.StatusCode = httpCode.ToInt();
            if(StatusCode>=300)
            {
                throw new Exception("错误的状态码！Success 结果只能是 “2xx” 状态码。");
            }
            this.Message = message;
        }
    }
}