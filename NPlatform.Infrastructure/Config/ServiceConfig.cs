/********************************************************************************

** auth： DongliangYi

** date： 2016/8/31 15:07:14

** desc： Memcached 配置文件实例

** Ver.:  V1.0.0

*********************************************************************************/

namespace NPlatform.Infrastructure.Config
{
    using Com.Ctrip.Framework.Apollo;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// 通用配置管理
    /// </summary>
    public class ServiceConfig : IServiceConfig
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// 数据中心ID
        /// </summary>
        [MaxLength(5,ErrorMessage ="数据中心ID长度不能超过5")]
        public long DataCenterID { get; set; } = 1;

        /// <summary>
        /// 分布式部署时服务ID
        /// </summary>
        [MaxLength(5, ErrorMessage = "服务ID长度不能超过5")]
        public long ServiceID { get; set; } = 1;

        /// <summary>
        /// 版本号
        /// </summary>
        public string ServiceVersion { get; set; } = "1.0";

        /// <summary>
        /// Gets or sets 需要注入的dll
        /// </summary>
        public string IOCAssemblys { get; set; }

        /// <summary>
        /// 系统地址,ip 或域名
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 系统的端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 可上传的附件
        /// </summary>
        public string AttachExtension { get; set; }

        public long AttachSize { get; set; }

        /// <summary>
        /// Gets or sets 主库连接
        /// </summary>
        public string MainConection { get; set; }

        /// <summary>
        /// Gets or sets 从库连接
        /// </summary>
        public string MinorConnection { get; set; }

        /// <summary>
        /// Gets or sets 数据库驱动
        /// </summary>
        public string DBProvider { get; set; }


        /// <summary>
        /// Gets or sets 事务超时时间,单位秒
        /// </summary>
        public int? TimeOut { get; set; }
    }
}