﻿/*******************************************************************************

**auth： DongliangYi

** date： 2016/8/29 10:08:46

* *desc：Dto接口

** Ver.:  V1.0.0

*********************************************************************************/
using NPlatform.Dto;
using NPlatform.Query;
using NPlatform.Result;
using System.ComponentModel.DataAnnotations;

namespace NPlatform.Extends
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
        /// <param name="newLineTag">错误消息换行符，默认br </param>
        /// <returns></returns>
        public static INPResult Validates(this IDto dto,string newLineTag="<br/>")
        {
            ValidationContext context = new ValidationContext(dto, serviceProvider: null, items: null);
            List<ValidationResult> results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(dto, context, results, true);

            if (isValid == false)
            {
                StringBuilder strErrors = new StringBuilder();
                foreach (var validationResult in results)
                {
                    strErrors.AppendLine($"{validationResult.ErrorMessage}{newLineTag}");
                }
                return new FailResult<IDto>(strErrors.ToString());
            }

            return new SuccessResult<IDto>(dto);
        }
        /// <summary>
        /// 校验查询条件是否合法,例如在service层的主动校验实体属性
        /// </summary>
        /// <param name="query">对象值</param>
        /// <returns></returns>
        public static INPResult Validates(this IQuery query)
        {
            ValidationContext context = new ValidationContext(query, serviceProvider: null, items: null);
            List<ValidationResult> results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(query, context, results, true);

            if (isValid == false)
            {
                StringBuilder strErrors = new StringBuilder();
                foreach (var validationResult in results)
                {
                    strErrors.AppendLine(validationResult.ErrorMessage);
                }
                return new FailResult<IQuery>(strErrors.ToString());
            }

            return new SuccessResult<IQuery>(query);
        }
    }
}
