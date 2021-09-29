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

    /// <summary>
    /// 通用配置管理
    /// </summary>
    public class ServiceConfig : IServiceConfig
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public long ServiceName { get; set; } = 1;

        /// <summary>
        /// 分布式部署时服务ID
        /// </summary>
        public long ServiceID { get; set; } = 1;

        /// <summary>
        /// 版本号
        /// </summary>
        public string ServiceVersion { get; set; }

        /// <summary>
        /// 需要注入的dll
        /// </summary>
        public string[] IOCAssemblys { get; set; }

        /// <summary>
        /// 系统地址
        /// </summary>
        public string ServiceEndPoint { get; set; }
    }
}