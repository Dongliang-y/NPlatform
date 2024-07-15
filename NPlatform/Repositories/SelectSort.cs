/**************************************************************
 *  Filename:    SelectSort.cs
 *  Copyright:   .
 *
 *  Description: SelectSort ClassFile.
 *
 *  @author:     Dongliang Yi
 *  @version     2021/9/8 16:52:38  @Reviser  Initial Version
 **************************************************************/

using NPlatform.Domains.Entity;

namespace NPlatform.Repositories
{
    /// <summary>
    /// 排序字段
    /// </summary>
    public interface ISelectSort<TEntity> where TEntity:IEntity
    {
        /// <summary>
        /// 字段名
        /// </summary>
        //
        // 摘要:
        //     字段名
        Expression<Func<TEntity, object>> FieldExp { get; set; }
        /// <summary>
        /// 字段名
        /// </summary>
        string FieldName { get; set; }
        /// <summary>
        /// 是否为 ASC排序
        /// </summary>
        bool IsAsc { get; set; }

    }

    /// <summary>
    /// 排序字段
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class SelectSort<TEntity> : ISelectSort<TEntity> where TEntity: IEntity
    {
        /// <summary>
        /// 字段名
        /// </summary>
        public Expression<Func<TEntity, object>> FieldExp { get; set; }

        /// <summary>
        ///  是否为 ASC排序
        /// </summary>
        public bool IsAsc { get; set; }

        /// <summary>
        /// 字段名
        /// </summary>
        public string FieldName { get; set; }
    }
}
