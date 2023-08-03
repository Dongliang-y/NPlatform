using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace NPlatform.Result
{
    /// <summary>
    /// 树类型的结构
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeResult<T> : List<T>, ITreeResult<T>, IStatusCodeActionResult
    {
        /// <summary>
        /// 树类型的结构
        /// </summary>
        public TreeResult() { }
        /// <summary>
        /// 树类型的结构
        /// </summary>
        public TreeResult(IEnumerable<T> treeNodes)
        {
            this.AddRange(treeNodes);
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
        public string? ContentType { get; set; } = HttpContentType.APPLICATION_JSON;
        /// <summary>
        /// 状态码
        /// </summary>
        [DataMember]
        public int? StatusCode { get; set; } = 200;

        /// <summary>
        /// 
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [JsonIgnore]
        public object Data { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        [JsonIgnore]
        public object SerializerSettings { get; set; }

        /// <inheritdoc />
        public async Task ExecuteResultAsync(ActionContext context)
        {
            await new JsonResult(this, SerializerSettings).ExecuteResultAsync(context);
        }
    }
}
