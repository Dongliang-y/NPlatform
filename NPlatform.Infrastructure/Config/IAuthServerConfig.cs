/*************************************************************************************
  * CLR版本：       4.0.30319.42000
  * 类 名 称：       AuthInfo
  * 机器名称：       DESKTOP123
  * 命名空间：       EPlatform.Infrastructure.Config
  * 文 件 名：       AuthInfo
  * 创建时间：       2020-3-24 9:25:28
  * 作    者：          xxx
  * 说   明：。。。。。
  * 修改时间：
  * 修 改 人：
*************************************************************************************/
namespace NPlatform.Config
{
    public interface IAuthServerConfig
    {
        string AccessTokenLifetime { get; set; }
        string[] ApiScope { get; set; }
        string AuthorizationUrl { get; set; }
    }
}