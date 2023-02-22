/********************************************************************************

** auth： DongliangYi

** date： 2016/8/29 10:08:46

** desc：Dto接口 基类

** Ver.:  V1.0.0

*********************************************************************************/
using System.ComponentModel.DataAnnotations;

namespace NPlatform.Dto
{
    /// <summary>
    /// Dto 基类
    /// </summary>
    public abstract class BaseDto : IDto
    {
        private IEnumerable<ValidationResult> _ValidationResult;
        public BaseDto(string aggregateId)
        {
            AggregateId = aggregateId;
        }
        public BaseDto()
        {
        }

        public string AggregateId { get; set; }

        /// <summary>
        /// 获取校验结果
        /// </summary>
        public virtual IEnumerable<ValidationResult> ValidationResult
        {
            get
            {
                return _ValidationResult;
            }
        }

        /// <summary>
        /// 执行校验
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            ValidationContext context = new ValidationContext(this, serviceProvider: null, items: null);
            List<ValidationResult> results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(this, context, results, true);
            _ValidationResult = results;
            return isValid;
        }
    }
}
