/********************************************************************************

** auth： DongliangYi

** date： 2016/8/31 15:07:14

** desc： Memcached 配置文件实例

** Ver.:  V1.0.0

*********************************************************************************/

namespace NPlatform.Infrastructure
{
    public interface IRedisConfig
    {
        bool AllowAdmin { get; set; }
        string[] Connections { get; set; }
        bool EnableCacheInterceptor { get; set; }
        string Password { get; set; }
        int Pipe { get; set; }
        string RedisType { get; set; }
        /// <summary>
        /// 当前服务的数据库序号
        /// </summary>
        int dbNum { get; set; }
    }
}