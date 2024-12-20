﻿#region << 版 本 注 释 >>

/*----------------------------------------------------------------
* 项目名称 ：NPlatform.Domains.Entity
* 类 名 称 ：IClient
* 类 描 述 ：客户端过滤器接口
* 命名空间 ：NPlatform.Domains.Entity
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Dongliang Yi
* 创建时间 ：2018-11-20 15:13:31
* 更新时间 ：2018-11-20 15:13:31
* 版 本 号 ：v1.0.0.0
//----------------------------------------------------------------*/

#endregion

namespace NPlatform.Filters
{
    /// <summary>
    /// 客户端
    /// </summary>
    public interface IClient : IFilterProperties
    {
        /// <summary>
        /// 客户端Id
        /// </summary>
        string ClientId { get; set; }
    }
}