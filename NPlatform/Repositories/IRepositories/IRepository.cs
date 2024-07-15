#region << 版 本 注 释 >>

/*----------------------------------------------------------------
* 项目名称 ：NPlatform.Domains.IUnitOfWork
* 类 名 称 ：IAggregationUnitOfWork
* 类 描 述 ：
* 命名空间 ：NPlatform.Domains.IUnitOfWork
* CLR 版本 ：4.0.30319.42000
* 作    者 ：DongliangYi
* 创建时间 ：2018-11-19 17:39:43
* 更新时间 ：2018-11-19 17:39:43
* 版 本 号 ：v1.0.0.0
//----------------------------------------------------------------*/

#endregion


namespace NPlatform.Repositories.IRepositories
{
    using NPlatform.Domains.Entity;
    using NPlatform.Repositories;
    using NPlatform.Result;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    /// <summary>
    /// 聚合内的工作单元接口
    /// 仓储的作用对象的领域模型的聚合根，也就是说每一个聚合都有一个单独的仓储
    /// </summary>
    /// <typeparam name="TEntity">实体类型
    /// </typeparam>
    /// <typeparam name="TPrimaryKey">主键类型
    /// </typeparam>
    public interface IRepository<TEntity, TPrimaryKey>
        where TEntity : EntityBase<TPrimaryKey>
    {
        /// <summary>
        /// this 重载
        /// </summary>
        /// <param name="key">主键</param>
        /// <returns>返回实体</returns>
        TEntity this[TPrimaryKey key] { get; set; }

        /// <summary>
        /// 异步新增
        /// </summary>
        /// <param name="items">实体对象</param>
        Task<int> AddsAsync(params TEntity[] items);

        /// <summary>
        /// 异步新增
        /// </summary>
        /// <param name="items">新增对象的集合</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task<int> AddOrUpdate(TEntity items);

        /// <summary>
        /// 异步修改
        /// </summary>
        /// <param name="item">修改的对象</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task<int> UpdateAsync(TEntity item);

        /// <summary>
        /// 聚合的删除方法，多半涉及某一个业务聚合的操作，
        /// 所以接口约束，基类使用抽象方法约束。
        /// </summary>
        /// <param name="filter">删除条件</param>
        /// <returns></returns>
        Task<int> RemoveAsync(Expression<Func<TEntity, bool>> filter);
        /// <summary>
        /// 聚合的删除方法，多半涉及某一个业务聚合的操作，
        /// 所以接口约束，基类使用抽象方法约束。
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        Task<int> RemoveAsync(params TPrimaryKey[] keys);

        /// <summary>
        /// 数据是否存在
        /// </summary>
        /// <param name="key">主键</param>
        Task<bool> ExistsAsync(TPrimaryKey key);

        /// <summary>
        /// 查询对象是否存在
        /// </summary>
        /// <param name="filter">筛选条件</param>
        /// <returns>结果</returns>
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter);


        /// <summary>
        /// 查找数据
        /// </summary>
        /// <param name="key">主键</param>
        Task<TEntity> FindByAsync(TPrimaryKey key);

        /// <summary>
        /// 异步查询所有数据
        /// </summary>
        /// <param name="sorts">排序字段</param>
        /// <returns>结果</returns>
        Task<IEnumerable<TEntity>> GetAllAsync(IEnumerable<SelectSort> sorts = null);

        /// <summary>
        /// 按条件查找第一个
        /// </summary>
        /// <param name="filter">条件</param>
        /// <returns>结果对象</returns>
        Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter);

        /// <summary>
        /// 根据表达式异步获取
        /// </summary>
        /// <param name="filter">表达式</param>
        /// <param name="sorts">排序</param>
        /// <returns>查询结果</returns>
        Task<IEnumerable<TEntity>> GetListByExpAsync(Expression<Func<TEntity, bool>> filter, IEnumerable<SelectSort> sorts = null);

        /// <summary>
        /// 指定字段范围查询，返回的实体只有这几个字段有值，目的是为了避免字段多时全字段查询（select *）
        /// </summary>
        /// <param name="columnNames">需要指定查询的字段</param>
        /// <param name="filter">筛选条件</param>
        /// <param name="sorts">排序字段</param>
        /// <returns>实体集合</returns>
        Task<IEnumerable<TEntity>> GetListWithColumnsAsync(IEnumerable<string> columnNames,
            Expression<Func<TEntity, bool>> filter,
            IEnumerable<SelectSort> sorts = null);

        /// <summary>
        /// 异步分页
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="filter">条件</param>
        /// <param name="sorts">排序</param>
        /// <returns>异步结果</returns>
        Task<IListResult<TEntity>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            Expression<Func<TEntity, bool>> filter,
            IEnumerable<SelectSort> sorts);


        #region 统计
        /// <summary>
        /// 统计记录数
        /// </summary>
        /// <param name="filter">linq表达式</param>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> filter);

        /// <summary>
        /// 求最大值
        /// </summary>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="selector">属性名</param>
        /// <param name="filter">过滤条件</param>
        /// <returns>最大值</returns>
        Task<TValue> MaxAsync<TValue>(Expression<Func<TEntity, TValue>> selector, Expression<Func<TEntity, bool>> filter = null);


        /// <summary>
        /// 求最小值
        /// </summary>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="selector">属性名</param>
        /// <param name="filter">过滤条件</param>
        /// <returns>最小值</returns>
        Task<TValue> MinAsync<TValue>(Expression<Func<TEntity, TValue>> selector, Expression<Func<TEntity, bool>> filter = null);


        /// <summary>
        /// 求和
        /// </summary>
        /// <param name="selector">属性名</param>
        /// <param name="filter">过滤条件</param>
        /// <returns>和</returns>
        Task<decimal> SumAsync(Expression<Func<TEntity, decimal>> selector, Expression<Func<TEntity, bool>> filter = null);

        /// <summary>
        /// 求平均值
        /// </summary>
        /// <param name="selector">属性名</param>
        /// <param name="filter">过滤条件</param>
        /// <returns>平均值</returns>
        Task<decimal> AVGAsync(Expression<Func<TEntity, decimal>> selector, Expression<Func<TEntity, bool>> filter = null);
        Task<int> SaveChangesAsync();
        #endregion

    }
}