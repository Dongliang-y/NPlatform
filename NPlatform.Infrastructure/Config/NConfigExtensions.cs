/**************************************************************
 *  Filename:    AppConfigService.cs
 *  Copyright:   .
 *
 *  Description: AppConfigService ClassFile.
 *
 *  @author:     Dongliang Yi
 *  @version     2021/9/23 16:50:09  @Reviser  Initial Version
 **************************************************************/
using NPlatform.Infrastructure.Config.Section;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace NPlatform.Infrastructure.Config
{
    public static class NConfigExtensions
    {

        /// <summary>
        /// 创建对象，只处理基本数据类型的字段，
        /// </summary>
        /// <typeparam name="T">要创建的类型</typeparam>
        /// <param name="values">字典</param>
        /// <param name="fun">委托，复杂类型交给调用方自己处理</param>
        /// <returns></returns>
        public static T ToObject<T>(this IConfiguration values, T model, Func<T, T> fun = null) where T : new()
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            var attr = typeof(T).GetProperties();

            if (model == null)
            {
                model = new T();
            }

            foreach (var prop in attr)
            {
                var val = values[prop.Name];
                if (val == null)
                {
                    continue;
                }

                if (prop.PropertyType == typeof(string))
                {
                    prop.SetValue(model, val.ToString());
                }
                else if (prop.PropertyType == typeof(long))
                {
                    prop.SetValue(model, Convert.ToInt64(val));
                }
                else if (prop.PropertyType == typeof(int))
                {
                    prop.SetValue(model, Convert.ToInt32(val));
                }
                else if (prop.PropertyType == typeof(bool))
                {
                    prop.SetValue(model, Convert.ToBoolean(val));
                }
                else if (prop.PropertyType == typeof(decimal))
                {
                    prop.SetValue(model, Convert.ToDecimal(val));
                }
                else if (prop.PropertyType == typeof(DateTime))
                {
                    prop.SetValue(model, Convert.ToDecimal(val));
                }
                else if (prop.PropertyType == typeof(float))
                {
                    prop.SetValue(model, Convert.ToDouble(val));
                }
                else if (prop.PropertyType == typeof(byte))
                {
                    prop.SetValue(model, Convert.ToByte(val));
                }
                else if (prop.PropertyType == typeof(double))
                {
                    prop.SetValue(model, Convert.ToDouble(val));
                }
            }

            model = fun != null ? fun(model) : model;
            return model;
        }

        /// <summary>
        /// redis 配置
        /// </summary>
        /// <returns></returns>
        public static IRedisConfig GetRedisConfig(this IConfiguration configuration)
        {
            // 检查配置是否为空
            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

            // 尝试从 Section 读取配置
            var section = configuration.GetSection(nameof(RedisConfig));
            if (section.Exists())
            {
                var redisConfig = new RedisConfig();
                section.Bind(redisConfig);

                // 检查从 Section 读取的配置是否有效
                if (IsValidRedisConfig(redisConfig))
                {
                    return redisConfig;
                }
            }

            // 如果 Section 配置无效，尝试从环境变量读取配置
            var host = configuration["REDIS_HOST"];
            var password = configuration["REDIS_PASSWORD"];
            var redisType = configuration["REDIS_REDISTYPE"];
            var dbNumString = configuration["REDIS_DBNUM"];
            if (string.IsNullOrWhiteSpace(dbNumString))
                dbNumString = "0";
            if (string.IsNullOrWhiteSpace(redisType))
                redisType = "Normal";
            // 检查环境变量是否有效
            if (!string.IsNullOrWhiteSpace(host) && int.TryParse(dbNumString, out int dbNum))
            {
                return new RedisConfig
                {
                    Connections = new[] { host },
                    Password = password,
                    dbNum = dbNum,
                    RedisType = redisType
                };
            }

            // 如果都没有有效的配置，返回 null
            return null;
        }

        // 辅助方法，用于检查 RedisConfig 是否有效
        private static bool IsValidRedisConfig(RedisConfig config)
        {
            return config != null && config.Connections != null && config.Connections.Length > 0;
        }

        public static IServiceConfig GetServiceConfig(this IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(nameof(configuration));
            ServiceConfig serviceConfig = new ServiceConfig();
            configuration.GetRequiredSection(nameof(ServiceConfig)).Bind(serviceConfig);
            return serviceConfig;
        }

        public static IAuthServerConfig GetAuthConfig(this IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(nameof(configuration));
            AuthServerConfig authConfig = new AuthServerConfig();
            configuration.GetRequiredSection(nameof(AuthServerConfig)).Bind(authConfig);
            return authConfig;
        }
    }
}
