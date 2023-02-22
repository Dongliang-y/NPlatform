// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace NPlatform.Filters
{
    /// <summary>
    /// ∑¿Œ±token–£—È
    /// </summary>
    public class ValidateAntiforgeryAuthorizationFilter : IAsyncAuthorizationFilter, IAntiforgeryPolicy
    {
        private readonly IAntiforgery _antiforgery;
        private readonly ILogger _logger;

        public ValidateAntiforgeryAuthorizationFilter(IAntiforgery antiforgery, ILoggerFactory loggerFactory)
        {
            if (antiforgery == null)
            {
                throw new ArgumentNullException(nameof(antiforgery));
            }

            _antiforgery = antiforgery;
            _logger = loggerFactory.CreateLogger(GetType());
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (!context.IsEffectivePolicy<IAntiforgeryPolicy>(this))
            {
                _logger.LogTrace($"Skipping the execution of current filter as its not the most effective filter implementing the policy {typeof(IAntiforgeryPolicy)}");
                return;
            }

            if (ShouldValidate(context))
            {
                try
                {
                    //if (!context.HttpContext.Request.Headers.ContainsKey(CommonConst.XSRFTOKEN))
                    //{
                    //    if (context.HttpContext.Request.Cookies.ContainsKey(CommonConst.XSRFTOKEN))
                    //    {
                    //        var cookie = context.HttpContext.Request.Cookies[CommonConst.XSRFTOKEN];
                    //        context.HttpContext.Request.Headers[CommonConst.XSRFTOKEN] = cookie;
                    //    }
                    //    else
                    //    {
                    //        throw new AntiforgeryValidationException($"»±…Ÿ {CommonConst.XSRFTOKEN} token ");
                    //    }
                    //}
                    await _antiforgery.ValidateRequestAsync(context.HttpContext);
                }
                catch (AntiforgeryValidationException exception)
                {

                    _logger.LogInformation($"AntiforgeryTokenInvalid Antiforgery token validation failed."+exception.Message, exception);
                    context.Result = new AntiforgeryValidationFailedResult();
                }
            }
        }

        protected virtual bool ShouldValidate(AuthorizationFilterContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var method = context.HttpContext.Request.Method;
            if (HttpMethods.IsGet(method) ||
                HttpMethods.IsHead(method) ||
                HttpMethods.IsTrace(method) ||
                HttpMethods.IsOptions(method))
            {
                return false;
            }

            // Anything else requires a token.
            return true;
        }
    }
}