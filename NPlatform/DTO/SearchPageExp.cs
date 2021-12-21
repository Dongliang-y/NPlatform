/**************************************************************
 *  Filename:    QueryWhere.cs
 *  Copyright:   .
 *
 *  Description: QueryWhere ClassFile.
 *
 *  @author:     Dongliang Yi
 *  @version     2021/12/13 18:09:32  @Reviser  Initial Version
 **************************************************************/
using NPlatform.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NPlatform.Dto
{
    /// <summary>
    /// 查询条件的封装
    /// </summary>
    public class SearchPageExp : SearchExp
    {

        /// <summary>
        /// 是否统计总数
        /// </summary>
        public bool CountTotal { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        [System.ComponentModel.DataAnnotations.Range(1,int.MaxValue)]
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; } = 10;
    }
}
