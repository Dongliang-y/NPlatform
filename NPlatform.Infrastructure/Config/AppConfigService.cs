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
using System.Text;

namespace NPlatform.Infrastructure.Config
{
    public class AppConfigService : IAppConfigService
    {
        IConfiguration configuration;
        public AppConfigService(IConfiguration config)
        {
            configuration = config;
        }

        /// <summary>
        /// redis 配置
        /// </summary>
        /// <returns></returns>
        public IRedisConfig GetRedisConfig()
        {
            var cfgRedis= configuration[nameof(RedisConfig)];
            IRedisConfig config = SerializerHelper.FromJson<RedisConfig>(cfgRedis);
            return config;
        }
        public IServiceConfig GetServiceConfig()
        {
            var cfgRedis = configuration[nameof(ServiceConfig)];
            IServiceConfig config = SerializerHelper.FromJson<ServiceConfig>(cfgRedis);
            return config;
        }

        public IAuthServerConfig GetAuthConfig()
        {
            var cfgRedis = configuration[nameof(AuthServerConfig)];
            IAuthServerConfig config = SerializerHelper.FromJson<AuthServerConfig>(cfgRedis);
            return config;
        }

    }
}
