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
        /// <summary>
        /// redis 配置
        /// </summary>
        /// <returns></returns>
        public static IRedisConfig GetRedisConfig(this IConfiguration configuration)
        {
            var cfgRedis= configuration[nameof(RedisConfig)];
            IRedisConfig config = SerializerHelper.FromJson<RedisConfig>(cfgRedis);
            return config;
        }
        public static IServiceConfig GetServiceConfig(this IConfiguration configuration)
        {
            var cfgRedis = configuration[nameof(ServiceConfig)];
            IServiceConfig config = SerializerHelper.FromJson<ServiceConfig>(cfgRedis);
            return config;
        }

        public static IAuthServerConfig GetAuthConfig(this IConfiguration configuration)
        {
            var cfgRedis = configuration[nameof(AuthServerConfig)];
            IAuthServerConfig config = SerializerHelper.FromJson<AuthServerConfig>(cfgRedis);
            return config;
        }

    }
}
