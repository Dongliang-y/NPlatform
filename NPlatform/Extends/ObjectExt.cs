/***********************************************************
**项目名称:
**功能描述: 仓储  的摘要说明
**作    者:   易栋梁
**版 本 号:    1.0
**创建日期： 2015/12/7 16:06:56
**修改历史：
************************************************************/

using NPlatform.Domains.Entity;

namespace NPlatform.Repositories {
    public static class ObjectExt
    {
        /// <summary>
        /// 创建对象，只处理基本数据类型的字段，
        /// </summary>
        /// <param name="entity">要创建的类型</param>
        /// <param name="src">源对象</param>
        /// <param name="fun">委托，复杂类型交给调用方自己处理</param>
        /// <returns></returns>
        public static TEntity CopyOrCreate<TEntity>(this TEntity entity, TEntity src, Func<TEntity, TEntity> fun = null) where TEntity: IEntity, new()
        {
            if (src == null)
            {
                throw new ArgumentNullException(nameof(src));
            }

            var attr = typeof(TEntity).GetProperties();

            if (entity == null)
                entity = new TEntity();
            foreach (var prop in attr)
            {
                var val = prop.GetValue(src);
                if (val == null)
                {
                    continue;
                }
                if (prop.PropertyType == typeof(short) || prop.PropertyType == typeof(short?))
                {
                    prop.SetValue(entity, Convert.ToInt16(val));
                }
                else if (prop.PropertyType == typeof(string))
                {
                    prop.SetValue(entity, val.ToString());
                }
                else if (prop.PropertyType == typeof(long) || prop.PropertyType == typeof(long?))
                {
                    prop.SetValue(entity, Convert.ToInt64(val));
                }
                else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
                {
                    prop.SetValue(entity, Convert.ToInt32(val));
                }
                else if (prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?))
                {
                    prop.SetValue(entity, Convert.ToBoolean(val));
                }
                else if (prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(decimal?))
                {
                    prop.SetValue(entity, Convert.ToDecimal(val));
                }
                else if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                {
                    prop.SetValue(entity, Convert.ToDateTime(val));
                }
                else if (prop.PropertyType == typeof(float) || prop.PropertyType == typeof(float?))
                {
                    prop.SetValue(entity, Convert.ToDouble(val));
                }
                else if (prop.PropertyType == typeof(byte) || prop.PropertyType == typeof(byte?))
                {
                    prop.SetValue(entity, Convert.ToByte(val));
                }
                else if (prop.PropertyType == typeof(double) || prop.PropertyType == typeof(double?))
                {
                    prop.SetValue(entity, Convert.ToDouble(val));
                }
            }
            entity = fun != null ? fun(entity) : entity;
            return entity;
        }
    }

}