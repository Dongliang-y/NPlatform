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
    public class ErrorResult<T> :ActionResult, IListResult<T>, ITreeResult<T>, INPResult
    {
        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="ex"></param>
        public ErrorResult(Exception ex) 
        {
            this.Message = ex.Message;
        }
        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="message"></param>
        public ErrorResult(string message) 
        {
            this.Message = message;
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="httpCode">http状态码</param>
        public ErrorResult(string message, HttpStatusCode httpCode)
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
        public  int? StatusCode { get; set; } = 500;

        /// <summary>
        /// Total，无需赋值
        /// </summary>
        [JsonIgnore]
        [System.Xml.Serialization.XmlIgnore]
        public long Total { get; }

        [JsonIgnore]
        public object Value { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


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
