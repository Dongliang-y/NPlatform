/********************************************************************************

** auth： DongliangYi

** date： 2016/8/31 15:07:14

** desc： Memcached 配置文件实例

** Ver.:  V1.0.0

*********************************************************************************/

namespace NPlatform.Infrastructure.Config
{
    public interface IServiceConfig
    {
        
        string ServiceName { get; set; }
        long DataCenterID { get; set; }
        long ServiceID { get; set; }
        string IOCAssemblys { get; set; }
        string ServiceVersion { get; set; }
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
    }
}