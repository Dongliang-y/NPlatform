/********************************************************************************

** auth： DongliangYi

** date： 2016/8/29 10:08:46

** desc：领域服务接口

** Ver.:  V1.0.0

*********************************************************************************/

namespace NPlatform.Domains.Service
{
    using NPlatform.Result;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// 领域服务接口
    /// </summary>
    public interface IDomainService<TDomainRoot, TPrimaryKey>
    {
        SuccessResult<TDomainRoot> Get(TPrimaryKey id);
        Task<INPResult> GetAllAsync();
        Task<INPResult> PostAsync(TDomainRoot entity);
        Task<INPResult> PutAsync(TDomainRoot entity);
        Task<INPResult> RemoveAsync(TPrimaryKey id);
    }
}