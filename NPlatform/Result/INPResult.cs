/********************************************************************************

** auth： DongliangYi

** date： 2017/8/24 12:34:28

** desc： 尚未编写描述

** Ver.:  V1.0.0

*********************************************************************************/
using System;
using System.Drawing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NPlatform.Infrastructure;

namespace NPlatform.Result
{
    /// <summary>
    /// 
    /// </summary>
    public interface INPResult
    {
        /// <summary>
        /// 服务实例ID
        /// </summary>
      //  string ServiceID { get; set; }
        /// <summary>
        /// Gets or sets 信息
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "message")]
        string Message { get; }

        /// <summary>
        /// 
        ///     Gets or sets the Microsoft.Net.Http.Headers.MediaTypeHeaderValue representing
        ///     the Content-Type header of the response.
        /// </summary>

        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code.
        /// </summary>
        public int? StatusCode { get; set; }

        /// <summary>
        /// 输出的数据
        /// </summary>
        public object Value { get; set; }
    }
}