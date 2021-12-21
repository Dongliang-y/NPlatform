/********************************************************************************

** auth： DongliangYi

** date： 2016/8/29 10:08:46

** desc：领域服务接口

** Ver.:  V1.0.0

*********************************************************************************/

namespace NPlatform.Domains.Service
{
    using NPlatform.Dto;
    using NPlatform.Repositories;
    using NPlatform.Result;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    /// <summary>
    /// 领域服务接口
    /// </summary>
    public interface IDomainService
    {
        /// <summary>
        /// 获取领域简称
        /// </summary>
        /// <returns></returns>
        public string GetDomainShortName();
    }
}