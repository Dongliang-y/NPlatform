/**************************************************************
 *  Filename:    QueryWhere.cs
 *  Copyright:   .
 *
 *  Description: QueryWhere ClassFile.
 *
 *  @author:     Dongliang Yi
 *  @version     2021/12/13 18:09:32  @Reviser  Initial Version
 **************************************************************/

using Newtonsoft.Json;
using NPlatform.Domains.Entity;
using NPlatform.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace NPlatform.Dto
{
    /// <summary>
    /// 查询条件的封装
    /// </summary>
    public class SearchExp:IDto
    {
        /// <summary>
        /// 查询条件
        /// </summary>
        [StringLength(1500)]
        public string Exp { get; set; }

        /// <summary>
        /// 排序条件
        /// </summary>
        [StringLength(1500)]
        [RegularExpression("^\\[(\\s)*\\{{1,}([\\s\\S]*)\\}{1,}(\\s)*\\]$", ErrorMessage = "排序条件必须是json格式的SelectSort对象结构，例如：[{\"field\":\"id\",\"isasc\":false},{\"field\":\"id\",\"isasc\":false}]")]
        public string SelectSorts { get; set; }

        /// <summary>
        /// 获取linq 格式的 的查询表达式
        /// </summary>
        /// <typeparam name="T">表达式查询的对象</typeparam>
        /// <returns></returns>
        public Expression<Func<T, bool>> GetExp<T>() where T : IEntity
        {
            if (this.Exp == null)
                return CreateExpression<T>();
            Expression<Func<T, bool>> exp = DynamicExpressionParser.ParseLambda<T, bool>(new ParsingConfig(), true, this.Exp);
            return exp;
        }


        /// <summary>
        /// 获取排序字段SelectSorts
        /// </summary>
        /// <typeparam name="T">表达式查询的对象</typeparam>
        /// <returns></returns>
        public IList<SelectSort> GetSelectSorts()
        {
            if (this.SelectSorts == null)
                return new List<SelectSort>() { new SelectSort() { Field="Id", IsAsc=true } };
            var sorts= JsonConvert.DeserializeObject<List<SelectSort>>(this.SelectSorts);
            return sorts;
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


    }
}
