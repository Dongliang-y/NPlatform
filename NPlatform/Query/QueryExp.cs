using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.RegularExpressions;
using NPlatform.Domains.Entity;
using NPlatform.Repositories;

namespace NPlatform.Query
{
    [ObsoleteAttribute("此类已过期，不可使用，请改用  DataSourceLoadOptionsBase 。", false)]
    public class QueryExp : IQuery
    {
        private string _LambdaExp;
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
        [RegularExpression(@"^\[(\s)*\{{1,}([\s\S]*)\}{1,}(\s)*\]$", ErrorMessage = "排序条件必须是json格式的SelectSort对象结构，例如：[{\"field\":\"id\",\"isasc\":false},{\"field\":\"id\",\"isasc\":false}]")]
        public string SelectSorts
        {
            get { return _SelectSorts; }
            set { _SelectSorts = value; }
        }

        private static readonly Regex SafeFieldNameRegex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$", RegexOptions.Compiled);

        private void ValidateFieldName<TEntity>(string fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldName) ||
                !SafeFieldNameRegex.IsMatch(fieldName) ||
                typeof(TEntity).GetProperty(fieldName) == null)
            {
                throw new ValidationException($"非法的字段名: {fieldName}");
            }
        }

        private void ValidateLambdaExp(string exp)
        {
            if (string.IsNullOrWhiteSpace(exp)) return;
            if (exp.Length > 1500) throw new ValidationException("表达式过长");
            string[] blackList = { "System.", "File", "Process", "Eval", "Invoke", "Dynamic", "`", "new", "typeof", "get_", "set_", "Add(" };
            foreach (var keyword in blackList)
            {
                if (exp.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    throw new ValidationException("表达式存在非法危险内容");
            }
            // 只允许符号等细致业务规则
        }

        /// <summary>
        /// 获取linq 格式的 的查询表达式
        /// </summary>
        public Expression<Func<TEntity, bool>> GetExp<TEntity>() where TEntity : EntityBase<string>
        {
            ValidateLambdaExp(this._LambdaExp);
            if (string.IsNullOrWhiteSpace(this._LambdaExp))
                return CreateExpression<TEntity>();

            // 可选：将字段名提取出来验证
            DynamicExpFieldSafetyCheck<TEntity>(this._LambdaExp);

            // 这里只能用信任的表达式解析库
            return DynamicExpressionParser.ParseLambda<TEntity, bool>(new ParsingConfig(), true, this._LambdaExp);
        }

        // 可按需自定义更完善的字段引用检查
        private void DynamicExpFieldSafetyCheck<TEntity>(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression)) return;

            // 简单从表达式中用正则提取的单词
            var matches = Regex.Matches(expression, @"\b[a-zA-Z_][a-zA-Z0-9_]*\b");
            foreach (Match match in matches)
            {
                var word = match.Value;
                // 可忽略常见操作符关键字
                if (new[] { "and", "or", "not", "true", "false" }.Contains(word.ToLower())) continue;

                // 检查是否为属性名，如果是，校验字段白名单
                if (typeof(TEntity).GetProperty(word) != null)
                {
                    ValidateFieldName<TEntity>(word);
                }
            }
        }

        public IList<SelectSort<TEntity>> GetSelectSorts<TEntity>() where TEntity : EntityBase<string>
        {
            if (string.IsNullOrWhiteSpace(this._SelectSorts))
                return null;

            var sorts = JsonSerializer.Deserialize<List<SelectSort<TEntity>>>(this._SelectSorts);
            var listSorts = new List<SelectSort<TEntity>>();
            foreach (var sort in sorts)
            {
                ValidateFieldName<TEntity>(sort.FieldName);
                listSorts.Add(new SelectSort<TEntity>
                {
                    FieldName = sort.FieldName,
                    IsAsc = sort.IsAsc,
                    FieldExp = CreateExpression<TEntity>(sort.FieldName)
                });
            }
            return listSorts;
        }

        private Expression<Func<TEntity, object>> CreateExpression<TEntity>(string propertyName)
        {
            ValidateFieldName<TEntity>(propertyName);
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var property = Expression.Property(parameter, propertyName);
            if (property.Type.IsGenericType && property.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var hasValue = Expression.Property(property, "HasValue");
                var valueProperty = Expression.Property(property, "Value");
                var nullConstant = Expression.Constant(null, typeof(object));
                var convertValue = Expression.Convert(valueProperty, typeof(object));
                var condition = Expression.Condition(hasValue, convertValue, nullConstant);
                return Expression.Lambda<Func<TEntity, object>>(condition, parameter);
            }
            return Expression.Lambda<Func<TEntity, object>>(property, parameter);
        }

        protected Expression<Func<TEntity, bool>> CreateExpression<TEntity>() where TEntity : EntityBase<string>
        {
            Expression<Func<TEntity, bool>> expression = t => t.Id != null;
            return expression;
        }
    }
}
