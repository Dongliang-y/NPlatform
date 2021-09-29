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
        
        long ServiceName { get; set; }
        long ServiceID { get; set; }
        string[] IOCAssemblys { get; set; }
        string ServiceVersion { get; set; }
        string ServiceEndPoint { get; set; }

    }
}