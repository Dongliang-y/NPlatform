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
        private string _LambdaExp;
       /// <summary>
       /// 查询条件,Lambda 表达式格式的条件
       /// </summary>
       [StringLength(1500)]
        public string LambdaExp
        {
            get { return _LambdaExp; }
            set { _LambdaExp = value; }
        }
        private string _SelectSorts;
        /// <summary>
        /// 排序条件
        /// </summary>
        [StringLength(1500)]
        [RegularExpression("^\\[(\\s)*\\{{1,}([\\s\\S]*)\\}{1,}(\\s)*\\]$", ErrorMessage = "排序条件必须是json格式的SelectSort对象结构，例如：[{\"field\":\"id\",\"isasc\":false},{\"field\":\"id\",\"isasc\":false}]")]
        public string SelectSorts
        {
            get { return _SelectSorts; }
            set { _SelectSorts = value; }
        }

        /// <summary>
        /// 获取linq 格式的 的查询表达式
        /// </summary>
        /// <typeparam name="T">表达式查询的对象</typeparam>
        /// <returns></returns>
        public Expression<Func<TEntity, bool>> GetExp<TEntity>() where TEntity : IEntity
        {
            if (this._LambdaExp == null)
                return CreateExpression<TEntity>();
            Expression<Func<TEntity, bool>> exp = DynamicExpressionParser.ParseLambda<TEntity, bool>(new ParsingConfig(), true, this._LambdaExp);
            return exp;
        }


        /// <summary>
        /// 获取排序字段SelectSorts
        /// </summary>
        /// <typeparam name="T">表达式查询的对象</typeparam>
        /// <returns></returns>
        public IList<SelectSort<TEntity>> GetSelectSorts<TEntity>() where TEntity : IEntity
        {
            if (this._SelectSorts == null)
                return null;
            var sorts = System.Text.Json.JsonSerializer.Deserialize<List<SelectSort<TEntity>>>(this._SelectSorts);
            var listSorts=new List<SelectSort<TEntity>>();
            foreach (var sort in sorts)
            {
                listSorts.Add(new SelectSort<TEntity>()
                {
                      FieldName = Safe.SqlFilterKeyword(Safe.FilterBadChar(sort.FieldName)),
                      IsAsc = sort.IsAsc,
                      FieldExp= CreateExpression<TEntity>(sort.FieldName)
                });

            }
            return listSorts;
        }
        private Expression<Func<TEntity, object>> CreateExpression<TEntity>(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var property = Expression.Property(parameter, propertyName);

            // 不进行类型转换，直接使用属性访问表达式作为 Lambda 表达式的主体
            var lambda = Expression.Lambda<Func<TEntity, object>>(property, parameter);
            return lambda;
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
