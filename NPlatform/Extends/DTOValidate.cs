/*******************************************************************************

**auth： DongliangYi

** date： 2016/8/29 10:08:46

* *desc：Dto接口

** Ver.:  V1.0.0

*********************************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NPlatform.Result;

namespace NPlatform
{
    /// <summary>
    /// Dto 校验扩展
    /// </summary>
    public static class DtoValidate
    {
        /// <summary>
        /// 校验模型的属性值是否合法,例如在service层的主动校验实体属性
        /// </summary>
        /// <param name="dto">对象值</param>
        /// <returns></returns>
        public static INPResult Validates(this IDto dto)
        {
            ValidationContext context = new ValidationContext(dto, serviceProvider: null, items: null);
            List<ValidationResult> results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(dto, context, results, true);

            if (isValid == false)
            {
                StringBuilder strErrors = new StringBuilder();
                foreach (var validationResult in results)
                {
                    strErrors.AppendLine(validationResult.ErrorMessage);
                }
                return new ErrorResult<IDto>(strErrors.ToString());
            }

            return new SuccessResult<IDto>(dto);
        }
    }
}
