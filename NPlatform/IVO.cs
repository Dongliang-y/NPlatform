/********************************************************************************

** auth： DongliangYi

** date： 2016/8/29 10:08:46

** desc：Dto接口

** Ver.:  V1.0.0

*********************************************************************************/

namespace NPlatform
{
    /// <summary>
    /// 1.viewmode  接口，当需要组合多个业务接口，呈现一个跨聚合、跨领域的试图时，无法用某个特定Dto描述时，可以创建VO对象。
    /// 2.VO对象以UI所需要的字段为准。
    /// 3.在MVC UI框架中存在于视图，在WEBAPI框架中，存在于特定的应用服务，比如“报表服务”。
    /// </summary>
    public interface IVO
    {
    }
}