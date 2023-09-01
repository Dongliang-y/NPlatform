#region << 版 本 注 释 >>

/*----------------------------------------------------------------
* 项目名称 ：NPlatform.Applications
* 项目描述 ：
* 类 名 称 ：Application
* 类 描 述 ：
* 所在的域 ：LDY
* 命名空间 ：NPlatform.Applications
* 机器名称 ：LDY 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：DongliangYi
* 创建时间 ：2018-12-21 10:32:12
* 更新时间 ：2018-12-21 10:32:12
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ DongliangYi 2018. All rights reserved.
*******************************************************************
//----------------------------------------------------------------*/

#endregion

namespace NPlatform.Applications
{
    using Microsoft.Extensions.Configuration;
    using NPlatform.AutoMap;
    using NPlatform.Result;
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Application 基类
    /// </summary>
    public abstract class ApplicationService : ResultHelper, IApplication
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
        public IMapperService MapperService { get; set; }


        public abstract string GetApplicationShortName();

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
                return Fail<T>("页码不能小于等于0");
            }
        }
        /// <summary>
        /// 创建某个实体对象的表达式
        /// </summary>
        /// <typeparam name="T">实体类型
        /// </typeparam>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        protected Expression<Func<T, bool>> CreateExpression<T>(Expression<Func<T, bool>> exp = null)
        {
            Expression<Func<T, bool>> expression;
            if (exp == null)
                return expression = x => 1 == 1;
            return exp;
        }
    }
}