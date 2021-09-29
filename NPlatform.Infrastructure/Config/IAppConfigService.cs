/**************************************************************
 *  Filename:    AppConfigService.cs
 *  Copyright:    Co., Ltd.
 *
 *  Description: AppConfigService ClassFile.
 *
 *  @author:     Dongliang Yi
 *  @version     2021/9/23 16:50:09  @Reviser  Initial Version
 **************************************************************/

namespace NPlatform.Infrastructure.Config
{
    public interface IAppConfigService
    {
        IAuthServerConfig GetAuthConfig();
        IRedisConfig GetRedisConfig();
        IServiceConfig GetServiceConfig();
    }
}