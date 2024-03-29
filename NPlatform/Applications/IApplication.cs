﻿/********************************************************************************

** auth： DongliangYi

** date： 2016/8/29 10:08:46

** desc：领域服务接口

** Ver.:  V1.0.0

*********************************************************************************/

namespace NPlatform.Applications
{
    /// <summary>
    /// application 层
    /// </summary>
    public interface IApplication
    {
        /// <summary>
        /// 获取应用简称
        /// </summary>
        /// <returns></returns>
        public string GetApplicationShortName();
    }
}