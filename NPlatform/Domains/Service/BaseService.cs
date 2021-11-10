namespace NPlatform.Domains.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using NPlatform.Domains.Entity;
    using NPlatform.IOC;
    using NPlatform.Repositories;
    using NPlatform.Result;
    using Flurl.Http;
    using NPlatform.Infrastructure.Config;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using NPlatform.Repositories.IRepositories;

    /// <summary>
    /// 领域服务基类,服务的对象是聚合，以聚合跟为核心。
    /// </summary>
    public abstract class BaseService<TDomainRoot,TPrimaryKey> : ResultBase, IDomainService<TDomainRoot,TPrimaryKey> 
        where TDomainRoot : EntityBase<TPrimaryKey>
    {
        /// <summary>
        /// 聚合根的仓储
        /// </summary>
        public IRepository<TDomainRoot,TPrimaryKey> Repository { get; set; }
        /// <summary>
        /// 框架配置
        /// </summary>
        public static IConfiguration Config { get; set; }

        /// <summary>
        /// httpContext
        /// </summary>
        public IPlatformHttpContext Context { get; set; }

        /// <summary>
        /// Domain service base。
        /// </summary>
        public BaseService()
        {
            IPlatformHttpContext httpCtx = Context;
            if (httpCtx != null && httpCtx.Context!=null)
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
        public IListResult<T> SearchArrayPage<T>(IEnumerable<T> sources, int page, int pageSize, out long total)
        {
            total = sources.Count();
            if (page > 0)
            {
                // 分页
                page--;
                var result = sources.Skip(page * pageSize).Take(pageSize).ToList();
                return ListData<T>( result,total);
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
            Expression<Func<TEntity, bool>> expression = t=>true;
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

        /// <summary>
        /// 创建对象
        /// </summary>
        /// <param name="entity">领域根对象</param>
        /// <returns></returns>
        public async Task<INPResult> PostAsync(TDomainRoot entity)
        {
            if(entity==null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            var rst= await Repository.AddAsync(entity);
            if(rst!=null)
            {
                return this.Success(rst);
            }
            else
            {
                return Error("新增失败");
            }
        }

        /// <summary>
        /// 修改对象
        /// </summary>
        /// <param name="entity">领域根对象</param>
        /// <returns></returns>
        public async Task<INPResult> PutAsync(TDomainRoot entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            var md = await Repository.FindByAsync(entity.Id);
            if (md == null)
            {
                return Error("指定的对象不存在！");
            }

            Repository[md.Id] = entity;

            return this.Success("修改成功");
        }

        /// <summary>
        /// 移除对象
        /// </summary>
        /// <param name="id">领域根对象ID</param>
        /// <returns></returns>
        public async Task<INPResult> RemoveAsync(TPrimaryKey id)
        {
            var rst = await Repository.RemoveAsync(id);
            return Success(rst);
        }


        /// <summary>
        /// 移除对象
        /// </summary>
        /// <param name="id">领域根对象ID</param>
        /// <returns></returns>
        public SuccessResult<TDomainRoot> Get(TPrimaryKey id)
        {
            var rst = Repository[id];
            return Success(rst);
        }

        /// <summary>
        /// 移除对象
        /// </summary>
        /// <param name="id">领域根对象ID</param>
        /// <returns></returns>
        public async Task<INPResult> GetAllAsync()
        {
            var rst = await Repository.GetAllAsync();
            return Success(rst);
        }
    }
}