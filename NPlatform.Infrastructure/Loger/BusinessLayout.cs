/*************************************************************************************
  * CLR版本：       4.0.30319.42000
  * 类 名 称：       BusinessLayout
  * 机器名称：       DESKTOP123
  * 命名空间：       NPlatform.Infrastructure.Loger
  * 文 件 名：       BusinessLayout
  * 创建时间：       2020-5-13 10:52:36
  * 作    者：          xxx
  * 说   明：。。。。。
  * 修改时间：
  * 修 改 人：
*************************************************************************************/
using log4net.Layout;

namespace NPlatform.Infrastructure.Loger
{
    /// <summary>
    /// 业务日志格式化
    /// </summary>
    public class BusinessLayout : PatternLayout
    {
        /// <summary>
        /// 业务日志格式化
        /// </summary>
        public BusinessLayout()
        {
            AddConverter("businessID", typeof(BusinessIDPatternConvert));
            AddConverter("creator", typeof(CreatorPatternConvert));
            AddConverter("message", typeof(MessagePatternConvert));
            AddConverter("module", typeof(ModulePatternConvert));
            AddConverter("logType", typeof(TypePatternConvert));
            AddConverter("logBrowser", typeof(LogBrowserPatternConvert));
            AddConverter("logLocation", typeof(LogLocationPatternConvert));
        }
    }
}
