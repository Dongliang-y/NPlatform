/**************************************************************
 *  Filename:    AppConfigService.cs
 *  Copyright:    Co., Ltd.
 *
 *  Description: AppConfigService ClassFile.
 *
 *  @author:     Dongliang Yi
 *  @version     2021/9/23 16:50:09  @Reviser  Initial Version
 **************************************************************/
using NPlatform.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace NPlatform.Infrastructure.Config
{
    public class AppConfigService : IAppConfigService
    {
        public IRedisConfig GetRedisConfig()
        {
            throw new NotImplementedException();
        }
        public IServiceConfig GetServiceConfig()
        {
            throw new NotImplementedException();
        }
        public IAuthServerConfig GetAuthConfig()
        {
            throw new NotImplementedException();
        }

    }
}
