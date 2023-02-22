/********************************************************************************

** auth： DongliangYi

** date： 2017/8/24 12:34:28

** desc： 尚未编写描述

** Ver.:  V1.0.0

*********************************************************************************/
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
namespace NPlatform.Result
{
    /// <summary>
    /// 
    /// </summary>
    public interface INPResult : IActionResult
    {
        /// <summary>
        /// 服务实例ID
        /// </summary>
      //  string ServiceID { get; set; }
        /// <summary>
        /// Gets or sets 信息
        /// </summary>
        [DataMember]
        [JsonPropertyName("message")]
        string Message { get; }

        /// <summary>
        /// Gets or sets the <see cref="Net.Http.Headers.MediaTypeHeaderValue"/> representing the Content-Type header of the response.
        /// </summary>
        public string? ContentType { get; set; }

        /// <summary>
        /// Gets or sets the serializer settings.
        /// <para>
        /// When using <c>System.Text.Json</c>, this should be an instance of <see cref="JsonSerializerOptions" />
        /// </para>
        /// <para>
        /// When using <c>Newtonsoft.Json</c>, this should be an instance of <c>JsonSerializerSettings</c>.
        /// </para>
        /// </summary>
        public object? SerializerSettings { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code. 严格执行 restfull 标准时推荐使用code来处理请求状态。
        /// </summary>
        public int? StatusCode { get; set; }

        /// <summary>
        /// 请求是否成功，是statusCode的封装成 bool类型，只关注请求结果是否成功。
        /// </summary>
        public bool Success
        {
            get
            {
                return StatusCode >= 200 && StatusCode < 300;
            }
        }
        /// <summary>
        /// 输出的数据
        /// </summary>
        public object Value { get; set; }
    }
}