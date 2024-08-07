﻿/********************************************************************************

** auth： DongliangYi

** date： 2016/8/31 15:07:14

** desc： Memcached 配置文件实例

** Ver.:  V1.0.0

*********************************************************************************/

namespace NPlatform.Infrastructure.Config
{
    public interface IServiceConfig
    {
        /// <summary>
        /// 服务名
        /// </summary>
        string ServiceName { get; set; }

        /// <summary>
        /// 数据中心ID
        /// </summary>
        string DataCenterID { get; set; }

        /// <summary>
        /// 服务ID
        /// </summary>
        string ServiceID { get; set; }

        /// <summary>
        /// 需要注入的dll
        /// </summary>
        string IOCAssemblys { get; set; }

        /// <summary>
        /// 服务版本
        /// </summary>
        string ServiceVersion { get; set; }

        /// <summary>
        /// 可上传的附件
        /// </summary>
        public string AttachExtension { get; set; }

        /// <summary>
        /// 上传大小限制
        /// </summary>
        public long AttachSize { get; set; }
    }
}