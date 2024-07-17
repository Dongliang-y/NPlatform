#region << 版 本 注 释 >>

/*----------------------------------------------------------------
* 项目名称 ：NPlatform.Domains.Entity
* 类 名 称 ：ITenant
* 类 描 述 ：是否是多租户过滤器接口
* 命名空间 ：NPlatform.Domains.Entity
* CLR 版本 ：4.0.30319.42000
* 作    者 ：DongliangYi
* 创建时间 ：2018-11-20 15:13:31
* 更新时间 ：2018-11-20 15:13:31
* 版 本 号 ：v1.0.0.0
//----------------------------------------------------------------*/

#endregion

using System.Threading.Tasks;

namespace NPlatform.Filters
{
    /// <summary>
    /// 多租户实体
    /// </summary>
    public interface ITenantEntity
    {
        /// <summary>
        /// 租户Id
        /// </summary>
        string TenantId { get; set; }
    }
}

//public class TenantMiddleware
//{
//    private readonly RequestDelegate _next;

//    public TenantMiddleware(RequestDelegate next)
//    {
//        _next = next;
//    }

//    public async Task InvokeAsync(HttpContext context)
//    {
//        // 从请求中获取租户ID
//        string tenantId = context.Request.Headers["TenantId"].ToString();

//        // 将租户ID存储在HttpContext.Items中
//        context.Items["TenantId"] = tenantId;

//        // 调用下一个中间件
//        await _next(context);
//    }
//}

//public void ConfigureServices(IServiceCollection services)
//{
//    // 注册IHttpContextAccessor服务
//    services.AddHttpContextAccessor();

//    // 其他服务配置...
//}

//public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
//{
//    // 注册租户中间件
//    app.UseMiddleware<TenantMiddleware>();

//    // 其他中间件配置...
//}

//public class MyRepository
//{
//    private readonly IHttpContextAccessor _httpContextAccessor;

//    public MyRepository(IHttpContextAccessor httpContextAccessor)
//    {
//        _httpContextAccessor = httpContextAccessor;
//    }

//    public void SomeMethod()
//    {
//        // 从HttpContext.Items中获取租户ID
//        string tenantId = _httpContextAccessor.HttpContext.Items["TenantId"].ToString();

//        // 使用租户ID
//        // ...
//    }
//}