/*************************************************************************************
  * CLR版本：       4.0.30319.42000
  * 类 名 称：       PlatformContext
  * 机器名称：       DESKTOP123
  * 命名空间：       NPlatform.Infrastructure
  * 文 件 名：       PlatformContext
  * 创建时间：       2020-5-21 9:19:34
  * 作    者：          xxx
  * 说   明：。。。。。
  * 修改时间：
  * 修 改 人：
*************************************************************************************/
using IdentityModel;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace NPlatform
{
    /// <summary>
    ///  平台  http 上下文
    /// </summary>
    public interface IPlatformHttpContext
    {
        /// <summary>
        /// http 上下文
        /// </summary>
        [JsonIgnore]
        HttpContext Context
        {
            get;
        }
        /// <summary>
        /// Claims
        /// </summary>
        [JsonIgnore]
        IEnumerable<Claim> Claims { get; }

        /// <summary>
        /// 用户ID
        /// </summary>
        string UserID { get; }

        /// <summary>
        /// 登录账号
        /// </summary>
        string Account { get; }

        /// <summary>
        /// 中文名
        /// </summary>
        string CName { get; }

        /// <summary>
        /// 昵称
        /// </summary>
        string NickName { get; }

        string ClientId { get; }

        /// <summary>
        /// 角色
        /// </summary>
        List<Claim> Roles { get; }

        /// <summary>
        /// 岗位
        /// </summary>
        List<Claim> Positions { get; }

        /// <summary>
        /// 所在机构ID
        /// </summary>
        string OrganizationId { get; }
    }
    /// <summary>
    /// 平台  http 上下文
    /// </summary>
    public class PlatformHttpContext : IPlatformHttpContext
    {
        /// <summary>
        ///  平台  http 上下文
        /// </summary>
        /// <param name="Accessor"></param>
        public PlatformHttpContext(IHttpContextAccessor Accessor)
        {
            _Accessor = Accessor;
        }
        private IHttpContextAccessor _Accessor;
        /// <summary>
        /// http 上下文
        /// </summary>
        [JsonIgnore]
        public HttpContext Context
        {
            get
            {
                return _Accessor.HttpContext;
            }
        }

        /// <summary>
        /// 登陆用户的附加信息
        /// </summary>
        [JsonIgnore]
        public IEnumerable<Claim> Claims
        {
            get
            {
                return _Accessor.HttpContext?.User?.Claims;
            }
        }

        /// <summary>
        /// 用户ID（登录后可用）
        /// </summary>
        public string UserID
        {
            get
            {
                var uid = Claims.FirstOrDefault(t => t.Type == JwtClaimTypes.Id);
                if(string.IsNullOrWhiteSpace(uid?.Value))
                {
                    uid = Claims.FirstOrDefault(t => t.Type == "sub");
                }
                return uid?.Value??"";
            }
        }

        /// <summary>
        /// 账号（登录后可用）
        /// </summary>
        public string Account
        {
            get
            {
                var account = Claims.FirstOrDefault(t =>t.Type == JwtClaimTypes.Name || t.Type == ClaimTypes.Name);
                return account != null ? account.Value : "";
            }
        }

        /// <summary>
        /// 大名
        /// </summary>
        public string CName
        {
            get
            {
                var CnName = Claims.FirstOrDefault(t => t.Type == JwtClaimTypes.GivenName||t.Type==ClaimTypes.GivenName);
                return CnName != null ? CnName.Value : "";
            }
        }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName
        {
            get
            {
                var CnName = Claims.FirstOrDefault(t => t.Type == JwtClaimTypes.NickName);
                return CnName != null ? CnName.Value : "";
            }
        }
        public string ClientId { get { 
             var clientId= Claims.FirstOrDefault(t => t.Type == JwtClaimTypes.ClientId);
                return clientId!=null? clientId.Value:"";
            } }

        /// <summary>
        /// 角色清单
        /// </summary>
        public List<Claim> Roles
        {
            get
            {
                var roles = Claims.Where(t => t.Type == JwtClaimTypes.Role);
                if(roles==null||roles.Count()==0)
                {
                    roles = Claims.Where(t => t.Type == ClaimTypes.Role).ToList();
                }
                return roles == null ? new List<Claim>() : roles.ToList();
            }
        }

        /// <summary>
        /// 所在机构ID
        /// </summary>
        public string OrganizationId
        {
            get
            {
                return Claims.FirstOrDefault(t => t.Type == "OrganizationId")?.Value;
            }
        }
        /// <summary>
        /// 岗位
        /// </summary>
        public List<Claim> Positions
        {
            get
            {
                var posts = Claims.Where(t => t.Type == "Posts");
                return posts == null ? new List<Claim>() : posts.ToList();
            }
        }
    }
}
