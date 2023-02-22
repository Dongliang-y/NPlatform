/********************************************************************************

** auth： DongliangYi

** date： 2017/8/23 13:21:01

** desc： 尚未编写描述

** Ver.:  V1.0.0

*********************************************************************************/
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace NPlatform.Result
{
    /// <summary>
    /// 数据列表内容对象
    /// </summary>
    [DataContract]
    public class ListResult<T> : IListResult<T>
    {

        /// <summary>
        /// 数据总数
        /// </summary>
        [DataMember]
        public long Total { get; }

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

        [DataMember]
        public IEnumerable<T> Value { get; set; }

        #region 不序列化返回的属性
        [JsonIgnore]
        [System.Xml.Serialization.XmlIgnore]
        object INPResult.Value { get { return this.Value; } set { this.Value = value as IEnumerable<T>; } }

        [JsonIgnore]
        [System.Xml.Serialization.XmlIgnore]
        public object SerializerSettings { get; set; }
        #endregion
        /// <summary>
        /// 数据列表内容对象
        /// </summary>
        public ListResult(IEnumerable<T> list, long total)
        {
            Total = total;
            this.Value = list;
        }
        /// <summary>
        /// 数据列表内容对象
        /// </summary>
        public ListResult(IEnumerable<T> list)
        {
            Total = list.Count();
            this.Value = list;
        }
        /// <summary>
        /// 数据列表内容对象
        /// </summary>
        public ListResult(IEnumerable<T> list, long total, int? httpCode)
        {
            Total = total;
            this.StatusCode = httpCode;
            this.Value = list;
        }

        /// <summary>
        /// 返回结果的集合
        /// </summary>
        /// <returns>结果集合</returns>
        public IList<T> ToList()
        {
            return this.Value.ToList();
        }

        /// <inheritdoc />
        public async Task ExecuteResultAsync(ActionContext context)
        {
            await new JsonResult(this, SerializerSettings).ExecuteResultAsync(context);
        }
    }
}