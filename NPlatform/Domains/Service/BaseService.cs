namespace NPlatform.Domains.Service
{
    using Flurl.Http;
    using Microsoft.Extensions.Configuration;
    using NPlatform.AutoMap;
    using NPlatform.Result;
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// 领域服务基类,服务的对象是聚合，以聚合跟为核心。
    /// </summary>
    public abstract class BaseService : ResultHelper, IDomainService
    {
        /// <summary>
        /// 框架配置
        /// </summary>
        [Autowired]
        public IConfiguration Config { get; set; }

        /// <summary>
        /// httpContext
        /// </summary>
        [Autowired]
        public IPlatformHttpContext Context { get; set; }
        /// <summary>
        /// mapper 对象
        /// </summary>
        [Autowired]
        public IMapperService mapperService { get; set; }

        /// <summary>
        /// Domain service base。
        /// </summary>
        public BaseService()
        {
            IPlatformHttpContext httpCtx = Context;
            if (httpCtx != null && httpCtx.Context != null)
            {
                var authorization = httpCtx.Context.Request.Headers["Authorization"];
                FlurlHttp.GlobalSettings.BeforeCall = t =>
                {
                    t.Request.Headers.Add("Authorization", authorization);
                    t.Request.Headers.Add("Accept", "application/json, text/plain, */*");
                };
            }
        }

        /// <summary>
        /// 集合分页
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="sources">数据源</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="total">总数</param>
        /// <returns>分页结果</returns>
        public IListResult<T> SearchArrayPage<T>(IQueryable<T> sources, int page, int pageSize, out long total)
        {
            total = sources.Count();
            if (page > 0)
            {
                // 分页
                page--;
                var result = sources.Skip(page * pageSize).Take(pageSize).ToList();
                return ListData<T>(result, total);
            }
            else
            {
                return Error<T>("页码不能小于等于0");
            }
        }
        /// <summary>
        /// 创建表达式
        /// </summary>
        /// <returns>表达式</returns>
        protected Expression<Func<TEntity, bool>> CreateExpression<TEntity>()
        {
            Expression<Func<TEntity, bool>> expression = t => true;
            return expression;
        }

        /// <summary>
        /// 创建表达式
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <returns>表达式</returns>
        protected Expression<Func<TEntity, bool>> CreateExpression<TEntity>(Expression<Func<TEntity, bool>> expression)
        {
            return expression;
        }

        public abstract string GetDomainShortName();
    }
}