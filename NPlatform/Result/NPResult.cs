/********************************************************************************

** auth： DongliangYi

** date： 2017/8/23 13:21:01

** desc： 尚未编写描述

** Ver.:  V1.0.0

*********************************************************************************/
using NPlatform.Extends;
using System.Net;
using System.Runtime.
Serialization;
using System.Text.Json.Serialization;

namespace NPlatform.Result
{

    /// <summary>
    /// 操作结果
    /// </summary>
    public class NPResult<T> : INPResult
    {
        /// <summary>
        /// 消息
        /// </summary>
        [DataMember]
        public string Message { get; set;}

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
        public int? StatusCode { get; set; }

        [DataMember]
        public object Data { get; set; }



        [JsonIgnore]
        [System.Xml.Serialization.XmlIgnore]
        public object? SerializerSettings { get; set; } = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.Never
        };

        /// <summary>
        /// 成功的结果内容
        /// </summary>
        public NPResult()
        {
        }

        /// <summary>
        /// 操作结果
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="data">T 类型对象</param>
        /// <param name="httpCode"></param>
        /// <param name="serializerSettings">序列化配置</param>
        public NPResult(string message, T data, HttpStatusCode httpCode, object? serializerSettings)
        {
            this.StatusCode = httpCode.ToInt();
            if (StatusCode >= 300)
            {
                throw new Exception("错误的状态码！Success 结果只能是 “2xx” 状态码。");
            }
            this.Message = message;
            this.SerializerSettings = serializerSettings;
        }
        public async Task ExecuteResultAsync(ActionContext context)
        {
            await new JsonResult(this, SerializerSettings).ExecuteResultAsync(context);
        }
    }
}