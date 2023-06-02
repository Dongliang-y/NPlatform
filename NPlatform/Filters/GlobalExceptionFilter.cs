using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using NPlatform.Result;

namespace NPlatform.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        readonly IWebHostEnvironment hostEnvironment;
        readonly ILogger<GlobalExceptionFilter> logger;
        public GlobalExceptionFilter(IWebHostEnvironment _hostEnvironment, ILogger<GlobalExceptionFilter> _logger)
        {
            this.hostEnvironment = _hostEnvironment;
            this.logger = _logger;
        }

        public void OnException(ExceptionContext context)
        {
            if (!context.ExceptionHandled)//如果异常没有处理
            {
                var result = new FailResult<string>("发生未处理异常", System.Net.HttpStatusCode.InternalServerError);

                if (hostEnvironment.IsDevelopment())
                {
                    result = new FailResult<string>(context.Exception);
                }

                logger.LogError(context.Exception.ToString());

                context.Result = new JsonResult(result);
                context.ExceptionHandled = true;//异常已处理
            }
        }
    }
}
