/**************************************************************
 *  Filename:    QueryWhere.cs
 *  Copyright:   .
 *
 *  Description: QueryWhere ClassFile.
 *
 *  @author:     Dongliang Yi
 *  @version     2021/12/13 18:09:32  @Reviser  Initial Version
 **************************************************************/

using IdentityModel;
using NPlatform.Domains.Entity;
using NPlatform.Repositories;
using System.ComponentModel.DataAnnotations;

namespace NPlatform.Query
{
    /// <summary>
    /// 查询条件的封装
    /// </summary>
    public class QueryExp : IQuery
    {
        private string lambdaExp;
       /// <summary>
       /// 查询条件,Lambda 表达式格式的条件
       /// </summary>
       [StringLength(1500)]
        public string LambdaExp
        {
            set
            {
                lambdaExp = value;
            }
        }

        private string selectSorts;
        /// <summary>
        /// 排序条件
        /// </summary>
        [StringLength(1500)]
        [RegularExpression("^\\[(\\s)*\\{{1,}([\\s\\S]*)\\}{1,}(\\s)*\\]$", ErrorMessage = "排序条件必须是json格式的SelectSort对象结构，例如：[{\"field\":\"id\",\"isasc\":false},{\"field\":\"id\",\"isasc\":false}]")]
        public string SelectSorts
        {
            set
            {
                selectSorts = value;
            }
        }

        /// <summary>
        /// 获取linq 格式的 的查询表达式
        /// </summary>
        /// <typeparam name="T">表达式查询的对象</typeparam>
        /// <returns></returns>
        public Expression<Func<TEntity, bool>> GetExp<TEntity>() where TEntity : IEntity
        {
            if (this.lambdaExp == null)
                return CreateExpression<TEntity>();
            Expression<Func<TEntity, bool>> exp = DynamicExpressionParser.ParseLambda<TEntity, bool>(new ParsingConfig(), true, this.lambdaExp);
            return exp;
        }


        /// <summary>
        /// 获取排序字段SelectSorts
        /// </summary>
        /// <typeparam name="T">表达式查询的对象</typeparam>
        /// <returns></returns>
        public IList<SelectSort<TEntity>> GetSelectSorts<TEntity>() where TEntity : IEntity
        {
            if (this.selectSorts == null)
                return null;
            var sorts = System.Text.Json.JsonSerializer.Deserialize<List<SelectSort<TEntity>>>(this.selectSorts);
            var listSorts=new List<SelectSort<TEntity>>();
            foreach (var sort in sorts)
            {
                listSorts.Add(new SelectSort<TEntity>()
                {
                      FieldName = sort.FieldName,
                       IsAsc = sort.IsAsc,
                });
            }
            return sorts;
        }

        /// <summary>
        /// 获取排序字段SelectSorts
        /// </summary>
        /// <typeparam name="T">表达式查询的对象</typeparam>
        /// <returns></returns>
        public string GetSelectSortsSql<TEntity>() where TEntity : IEntity
        {
            if (this.selectSorts == null)
                return null;
            var sorts = System.Text.Json.JsonSerializer.Deserialize<List<SelectSort<TEntity>>>(this.selectSorts);
            var listSorts = new List<SelectSort<TEntity>>();
            StringBuilder strExp = new StringBuilder();
            for(var i=0;i<sorts.Count;i++)
            {
                var sort = sorts[i];
                var asc = sort.IsAsc ? " asc" : " desc";
                strExp.Append($"{Safe.SqlFilterKeyword( Safe.FilterBadChar( sort.FieldName))}{asc}");

                if (i != sorts.Count - 1)
                    strExp.Append(",");
            }
            return strExp.ToString();
        }
        /// <summary>
        /// 创建表达式
        /// </summary>
        /// <returns>表达式</returns>
        protected Expression<Func<TEntity, bool>> CreateExpression<TEntity>() where TEntity : IEntity
        {
            Expression<Func<TEntity, bool>> expression = t => true;
            return expression;
        }


    }
}
