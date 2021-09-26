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
    public class ErrorResult<T> : IListResult<T>, ITreeResult<T>, INPResult
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
        public ErrorResult(string message,HttpStatusCode httpCode)
        {
            this.Message = message;
            this.HttpCode = httpCode;
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="ex">excpetion</param>
        /// <param name="httpCode">httpcode</param>
        /// <param name="message">message</param>
        public ErrorResult(Exception ex, HttpStatusCode httpCode)
        {
            this.Message = ex.Message;
            this.HttpCode = httpCode;
        }


        /// <summary>
        /// 消息
        /// </summary>
        [DataMember]
        [JsonPropertyName("message")]
        public string Message { get; }

        /// <summary>
        /// HTTP状态码
        /// </summary>
        [DataMember]
        [JsonPropertyName("httpcode")]
        public HttpStatusCode HttpCode { get;  } = HttpStatusCode.InternalServerError;

        /// <summary>
        /// 服务ID
        /// </summary>
        [DataMember]
        [JsonPropertyName("serviceid")]
        public string ServiceID { get; set; }


        /// <summary>
        /// 数据，无需赋值
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [JsonIgnore]
        public T Data { get; }

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
        IEnumerable<T> IListResult<T>.Data { get; } = null;

        public IList<T> ToList()
        {
            throw new NotImplementedException();
        }
    }
}
