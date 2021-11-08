/***********************************************************
**项目名称:	                                                                  				   
**功能描述:	  的摘要说明
**作    者: 	易栋梁                                         			   
**版 本 号:	1.0                                             			   
**创建日期： 2015/12/10 9:45:22
**修改历史：
************************************************************/

namespace NPlatform.Domains.IRepositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using NPlatform.Domains.Entity;

    /// <summary>
    /// 工作单元模式仓储接口
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// 事物是否已提交
        /// </summary>
        bool IsCommitted { get; }

        /// <summary>
        /// 获取/设置工作单元的超时时间
        /// </summary>
        int? Timeout { get; set; }

        /// <summary>
        /// 新增
        /// </summary>
        /// <typeparam name="T">需要新增的类型</typeparam>
        /// <param name="entity">实体对象</param>
        /// <returns>返回T</returns>
        Task<T> AddAsync<T>(T entity)
            where T : class, IEntity;

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="entitys">实体集合</param>
        Task<int> AddsAsync<T>(IEnumerable<T> entitys) where T : class, IEntity;

        /// <summary>
        /// 修改
        /// </summary>
        /// <typeparam name="T">需要修改的类型</typeparam>
        /// <param name="entity">实体对象</param>
        /// <returns>返回T</returns>
        Task<int> ChangeAsync<T>(T entity)
            where T : class, IEntity;

        /// <summary>
        /// 移除实体对象
        /// </summary>
        /// <typeparam name="T">需要新增的类型</typeparam>
        /// <param name="entities">实体对象</param>
        /// <returns>返回T</returns>
        Task<int> RemoveAsync<T>(params T[] entities)
            where T : class, IEntity;

        /// <summary>
        /// 移除对象
        /// </summary>
        /// <param name="filter"></param>
        Task<int> RemoveAsync<T>(Expression<Func<T, bool>> filter)
            where T : class, IEntity;

        /// <summary>
        /// 执行sql脚本
        /// </summary>
        /// <typeparam name="sql">需要执行的SQL</typeparam>
        /// <param name="parameters">参数对象</param>
        /// <returns>执行结果</returns>
        Task<IEnumerable<T>> QueryFromSql<T>(string sql, object parameters) where T : class, IEntity;

        /// <summary>
        /// 提交
        /// </summary>
        void Commit();

        /// <summary>
        /// 回滚事物
        /// </summary>
        void Rollback();
    }
}