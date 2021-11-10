using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NPlatform.Middleware
{
    /// <summary>
    /// 健康检查的用的中间件
    /// </summary>
    public class NHealthChecks : IHealthCheck
    {
        /// <summary>
        /// 健康检测
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            var kvs = new Dictionary<string, object>();
            kvs.Add("CheckTime",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            var dic = new ReadOnlyDictionary<string, object>(kvs);
            HealthCheckResult healthCheckResult = HealthCheckResult.Healthy("Check", dic);
            // 这里可以去检查下 数据库链接、redis等情况

            return Task.FromResult(healthCheckResult);
        }

    }
}
