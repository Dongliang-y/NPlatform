using NPlatform.Extends;
using System.Net;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace NPlatform.Result
{
    /// <summary>
    /// 错误信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FailResult<T> : IListResult<T>, ITreeResult<T>, INPResult
    {
        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="ex"></param>
        public FailResult(Exception ex)
        {
            this.Message = ex.Message;
        }
        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="message"></param>
        public FailResult(string message)
        {
            this.Message = message;
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="data">附加数据</param>
        public FailResult(string message,T data)
        {
            this.Message = message;
            this.Data = data;
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="httpCode">http状态码</param>
        public FailResult(string message, HttpStatusCode httpCode)
        {
            this.Message = message;
            this.StatusCode = httpCode.ToInt();
        }
        /// <summary>
        /// 操作结果
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="httpCode"></param>
        /// <param name="serializerSettings">序列化配置</param>
        public FailResult(string message, HttpStatusCode httpCode, object? serializerSettings)
        {
            this.StatusCode = httpCode.ToInt();
            if (StatusCode >= 300)
            {
                throw new Exception("错误的状态码！Success 结果只能是 “2xx” 状态码。");
            }
            this.Message = message;
            this.SerializerSettings = serializerSettings;
        }

        /// <summary>
        /// 消息
        /// </summary>
        [DataMember]
        public string Message { get; }

        /// <summary>
        ///  返回结果的服务id
        /// </summary>
        [DataMember]
        public string ServiceID { get; set; }

        /// <summary>
        ///  http heard contentType
        /// </summary>
        [DataMember]
        public string ContentType { get; set; } = HttpContentType.APPLICATION_JSON;
        /// <summary>
        /// 状态码
        /// </summary>
        [DataMember]
        public int? StatusCode { get; set; } = 500;

        /// <inheritdoc />
        public async Task ExecuteResultAsync(ActionContext context)
        {
            await new JsonResult(this, SerializerSettings).ExecuteResultAsync(context);
        }
        /// <summary>
        /// 附加数据
        /// </summary>
        public object Data { get; set; }

        #region 不序列化返回的属性
        /// <summary>
        /// Total，无需赋值
        /// </summary>
        [JsonIgnore]
        [System.Xml.Serialization.XmlIgnore]
        public long Total { get; }

        [JsonIgnore]
        [System.Xml.Serialization.XmlIgnore]
        public object SerializerSettings { get; set; }


        /// <summary>
        /// Total，无需赋值
        /// </summary>
        [JsonIgnore]
        [System.Xml.Serialization.XmlIgnore]
        IEnumerable<T> IListResult<T>.Data { get; } = null;

        /// <summary>
        /// NotImplementedException
        /// </summary>
        /// <returns></returns>
        public IList<T> ToList()
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
