using Microsoft.AspNetCore.Mvc;
using NPlatform.Infrastructure;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace NPlatform.Result
{
    /// <summary>
    /// 错误信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ErrorResult<T> :JsonResult, IListResult<T>, ITreeResult<T>, INPResult
    {
        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="ex"></param>
        public ErrorResult(Exception ex) : base(ex)
        {
            this.Message = ex.Message;
        }
        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="message"></param>
        public ErrorResult(string message) : base(null)
        {
            this.Message = message;
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="httpCode">http状态码</param>
        public ErrorResult(string message, HttpStatusCode httpCode) :base(null)
        {
            this.Message = message;
            this.StatusCode = httpCode.ToInt();
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="ex">excpetion</param>
        /// <param name="httpCode">httpcode</param>
        /// <param name="serializerSettings"></param>
        /// <param name="message">message</param>
        public ErrorResult(string message, Exception ex, HttpStatusCode httpCode,object serializerSettings) :base(ex,serializerSettings)
        {
            this.Message = message;
            this.StatusCode = httpCode.ToInt();
        }

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
        public new int? StatusCode { get; set; } = 500;

        /// <summary>
        /// Total，无需赋值
        /// </summary>
        [JsonIgnore]
        [System.Xml.Serialization.XmlIgnore]
        public long Total { get; }


        /// <summary>
        /// Total，无需赋值
        /// </summary>
        [JsonIgnore]
        [System.Xml.Serialization.XmlIgnore]
        IEnumerable<T> IListResult<T>.Value { get; } = null;

        /// <summary>
        /// NotImplementedException
        /// </summary>
        /// <returns></returns>
        public IList<T> ToList()
        {
            throw new NotImplementedException();
        }
    }
}
