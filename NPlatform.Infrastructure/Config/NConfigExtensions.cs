/**************************************************************
 *  Filename:    AppConfigService.cs
 *  Copyright:    Co., Ltd.
 *
 *  Description: AppConfigService ClassFile.
 *
 *  @author:     Dongliang Yi
 *  @version     2021/9/23 16:50:09  @Reviser  Initial Version
 **************************************************************/
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace NPlatform.Infrastructure.Config
{
    public static class NConfigExtensions
    {

        private static T CreateFromConfig<T>(IConfigurationSection configSection) where T: new()
        {
            var attr = typeof(T).GetProperties();
            T config = new T();
            foreach (var prop in attr)
            {
                var val =configSection[prop.Name];
                if (val == null)
                {
                    continue;
                }
                if (prop.PropertyType == typeof(long))
                {
                    prop.SetValue(config, Convert.ToInt64(val));
                }
                else if (prop.PropertyType == typeof(int))
                {
                    prop.SetValue(config, Convert.ToInt32(val));
                }
                else if (prop.PropertyType == typeof(string))
                {
                    prop.SetValue(config, val.ToString());
                }
                else if (prop.PropertyType == typeof(bool))
                {
                    prop.SetValue(config, Convert.ToBoolean(val));
                }
                else if (prop.PropertyType == typeof(decimal))
                {
                    prop.SetValue(config, Convert.ToDecimal(val));
                }
                else if (prop.PropertyType == typeof(DateTime))
                {
                    prop.SetValue(config, Convert.ToDecimal(val));
                }
                else
                {
                    prop.SetValue(config, val);
                }
            }
            return config;
        }

        /// <summary>
        /// redis 配置
        /// </summary>
        /// <returns></returns>
        public static IRedisConfig GetRedisConfig(this IConfiguration configuration)
        {
            var redisSection = configuration[nameof(RedisConfig)];
            IRedisConfig config = SerializerHelper.FromJson<RedisConfig>(redisSection);
            return config;
        }
        public static IServiceConfig GetServiceConfig(this IConfiguration configuration)
        {
            var serviceSection = configuration.GetSection(nameof(ServiceConfig));
            return CreateFromConfig<ServiceConfig>(serviceSection);
        }
        public static IAuthServerConfig GetAuthConfig(this IConfiguration configuration)
        {
            var authSection = configuration.GetSection(nameof(AuthServerConfig));
            return  CreateFromConfig<AuthServerConfig>(authSection);
        }

    }
}
