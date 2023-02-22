/********************************************************************************

** auth： DongliangYi

** date： 2016/8/31 15:07:14

** desc： Memcached 配置文件实例

** Ver.:  V1.0.0

*********************************************************************************/

namespace NPlatform.Infrastructure.Config.Section
{
    public sealed class RedisConfig : IRedisConfig
    {
        private string redisType;

        /// <summary>
        /// Gets or sets redis 安装模式，Normal 普通，Twemproxy 代理，Sentinel 哨兵
        /// </summary>
        public string RedisType { get; set; }

        private string[] connections;
        /// <summary>
        /// Gets or sets 连接字符串  ,  [IP:port]
        /// </summary>

        public string[] Connections { get; set; }

        private string password;
        /// <summary>
        /// redis 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// AllowAdmin
        /// </summary>
        public bool AllowAdmin { get; set; }
        /// <summary>
        /// redis 管道
        /// </summary>
        public int Pipe { get; set; }

        /// <summary>
        /// 是否开启缓存方法拦截
        /// </summary>
        public bool EnableCacheInterceptor { get; set; }

        public int dbNum { get; set; }
    }
}