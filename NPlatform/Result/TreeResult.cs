using Microsoft.AspNetCore.Mvc;
using NPlatform.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NPlatform.Result
{
    /// <summary>
    /// 树类型的结构
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeResult<T> : List<T>, ITreeResult<T>
    {
        /// <summary>
        /// 树类型的结构
        /// </summary>
        public TreeResult() { }
        /// <summary>
        /// 树类型的结构
        /// </summary>
        public TreeResult(IEnumerable<T> treeNodes) {
            this.AddRange(treeNodes);
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
        public string ContentType { get; set; } = HttpContentType.APPLICATION_JSON;
        /// <summary>
        /// 状态码
        /// </summary>
        public int? StatusCode { get; set; } = 200;

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public object Value { get; set; }
    }
}
