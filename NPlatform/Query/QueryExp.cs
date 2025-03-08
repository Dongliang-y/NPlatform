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
        public Expression<Func<TEntity, bool>> GetExp<TEntity>() where TEntity : EntityBase<string>
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
        public IList<SelectSort<TEntity>> GetSelectSorts<TEntity>() where TEntity :  EntityBase<string>
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

        // 创建 Expression<Func<TEntity, object>> 表达式
        private Expression<Func<TEntity, object>> CreateExpression<TEntity>(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var property = Expression.Property(parameter, propertyName);

            // 处理可为空类型的情况
            if (property.Type.IsGenericType && property.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // 获取可为空类型的基础类型
                var underlyingType = Nullable.GetUnderlyingType(property.Type);
                // 创建一个条件表达式，用于处理可为空类型的值
                var hasValueProperty = Expression.Property(property, "HasValue");
                var valueProperty = Expression.Property(property, "Value");
                var nullConstant = Expression.Constant(null, typeof(object));
                var convertValue = Expression.Convert(valueProperty, typeof(object));
                var condition = Expression.Condition(hasValueProperty, convertValue, nullConstant);

                return Expression.Lambda<Func<TEntity, object>>(condition, parameter);
            }

            // 非可为空类型，直接返回属性访问表达式
            return Expression.Lambda<Func<TEntity, object>>(property, parameter);
        }

        /// <summary>
        /// 创建表达式
        /// </summary>
        /// <returns>表达式</returns>
        protected Expression<Func<TEntity, bool>> CreateExpression<TEntity>() where TEntity : EntityBase<string>
        {
            Expression<Func<TEntity, bool>> expression = t =>t.Id!=null;
            return expression;
        }


    }
}
