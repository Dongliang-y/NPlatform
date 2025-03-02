/**************************************************************
 *  Filename:    QueryWhere.cs
 *  Copyright:   .
 *
 *  Description: QueryWhere ClassFile.
 *
 *  @author:     Dongliang Yi
 *  @version     2021/12/13 18:09:32  @Reviser  Initial Version
 **************************************************************/

using System.ComponentModel.DataAnnotations;
using NPlatform.Domains.Entity;
using NPlatform.Repositories;

namespace NPlatform.Query
{
    /// <summary>
    /// 查询条件的封装
    /// </summary>
    public class QueryPageExp : QueryExp
    {
        private string lambdaExp;
        /// <summary>
        /// 查询条件,Lambda 表达式格式的条件
        /// </summary>
        [StringLength(1500)]
        public new string LambdaExp
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
        public new string SelectSorts
        {
            set
            {
                selectSorts = value;
            }
        }
        /// <summary>
        /// 是否统计总数
        /// </summary>
        public bool CountTotal { get; set; } = true;

        /// <summary>
        /// 页码
        /// </summary>
        [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue)]
        public int PageNum { get; set; } = 1;

        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; } = 10;
    }
}
