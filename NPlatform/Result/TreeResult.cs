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
        public int? StatusCode { get; set; } = 200;

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public object Value { get; set; }


        /// <summary>
        /// Executes the result operation of the action method asynchronously. This method is called by MVC to process
        /// the result of an action method.
        /// The default implementation of this method calls the <see cref="ExecuteResult(ActionContext)"/> method and
        /// returns a completed task.
        /// </summary>
        /// <param name="context">The context in which the result is executed. The context information includes
        /// information about the action that was executed and request information.</param>
        /// <returns>A task that represents the asynchronous execute operation.</returns>
        public virtual Task ExecuteResultAsync(ActionContext context)
        {
            ExecuteResult(context);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Executes the result operation of the action method synchronously. This method is called by MVC to process
        /// the result of an action method.
        /// </summary>
        /// <param name="context">The context in which the result is executed. The context information includes
        /// information about the action that was executed and request information.</param>
        public virtual void ExecuteResult(ActionContext context)
        {
        }
    }
}
