/**************************************************************
 *  Filename:    SelectSort.cs
 *  Copyright:   .
 *
 *  Description: SelectSort ClassFile.
 *
 *  @author:     Dongliang Yi
 *  @version     2021/9/8 16:52:38  @Reviser  Initial Version
 **************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NPlatform.Repositories
{
    /// <summary>
    /// 排序字段
    /// </summary>
    public interface ISelectSort
    {
        /// <summary>
        /// 字段名
        /// </summary>
        string Field { get; set; }

        /// <summary>
        /// 是否为 ASC排序
        /// </summary>
        bool IsAsc { get; set; }
    }

    /// <summary>
    /// 排序字段
    /// </summary>
    public class SelectSort : ISelectSort
    {
        /// <summary>
        /// 字段名
        /// </summary>
        public string Field { get; set; }
        /// <summary>
        /// 是否为 ASC排序
        /// </summary>
        public bool IsAsc { get; set; }
    }
}
