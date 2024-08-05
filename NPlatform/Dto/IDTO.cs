/********************************************************************************

** auth： DongliangYi

** date： 2016/8/29 10:08:46

** desc：Dto接口

** Ver.:  V1.0.0

*********************************************************************************/

using System.ComponentModel.DataAnnotations;

namespace NPlatform.Dto
{
    /// <summary>
    /// Dto 接口，DTO用来处理层与层以及GRPC 服务之间的数据传输。
    /// 命令对象实现的DTO
    /// 领域返回的是DTO。
    /// 注意VO与DTO的区分，当需要组合多个DTO做汇总统计，或者DTO无法满足UI设计时，需要重新设计VO来包装DTO。
    /// </summary>
    public interface IDto
    {
        /// <summary>
        /// 聚合ID
        /// </summary>
        public string Id { get; set; }

        //DTO绑定验证，使用Fluent API来实现
        public IEnumerable<ValidationResult> ValidationResult { get; }


        //实现Command抽象类的DTO数据验证
        public bool IsValid();
    }
}