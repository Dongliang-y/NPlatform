using System;
using System.Collections.Generic;
using System.Linq;
using NPlatform.Result;
using NPlatform.Config;
using Microsoft.AspNetCore.Authorization;
using System.Web;
using NPlatform.Infrastructure.Loger;

namespace NPlatform.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Primitives;
    using ServiceStack;
    using System.IO;
    using System.Net.Http;
    using System.Text;
    using NPlatform.Infrastructure;
    using NPlatform.IOC;
    using NPlatform.Repositories;
    using System.Threading.Tasks;
    using NPlatform.Infrastructure.Config;
    using System.Net;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// controler 基类
    /// </summary>
    // [AllowAnonymous]
    [ApiController]
    [Authorize]
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// redis service
        /// </summary>
        RedisService _RedisService { get; set; }
        /// <summary>
        /// 全局配置信息
        /// </summary>
        public IAppConfigService Config { get; set; }

        public ILogger Logger { get; set; }

        public BaseController(IAppConfigService config, RedisService service,ILogger loger)
        {
            Config = config;
            _RedisService = service;
            this.Logger = loger;
        }


        /// <summary>
        /// 获取UI传递的js 数组参数 'Content-Type':'application/json' params:JSON.stringify(Array)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected async Task<T> GetRequestParamsAsync<T>()
        {
            using (var ms = new MemoryStream())
            {
                await Request.Body.CopyToAsync(ms);
                var b = ms.ToArray();
                var postParamsString = Encoding.UTF8.GetString(b);
                return NPlatform.Infrastructure.SerializerHelper.FromJson<T>(postParamsString);
            }
        }

        /// <summary>
        /// 获取UI传递的js 数组参数 'Content-Type':'application/json' params:JSON.stringify(Array)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected async virtual Task<string> GetRequestStrParamsAsync()
        {
            using (var ms = new MemoryStream())
            {
                await Request.Body.CopyToAsync(ms);
                var b = ms.ToArray();
                var postParamsString = Encoding.UTF8.GetString(b);
                return postParamsString;
            }
        }


        private SesstionInfo sesstion = null;

        /// <summary>
        /// 获取认证的身份信息
        /// </summary>
        protected virtual async Task<SesstionInfo> GetSesstionInfo()
        {
            try
            {
                if (this.sesstion == null)
                {
                    var token = this.Request.Headers["Authorization"];
                    sesstion = await _RedisService.StringGetAsync<SesstionInfo>(CommonRedisConst.SesstionKey(token));
                    if (sesstion == null)
                    {
                        var Claims = User.Claims;
                        sesstion = new SesstionInfo();
                        sesstion.AccessToken = token;
                        if (Claims.Any(t => t.Type == "id"))
                        {
                            sesstion.Id = Claims.FirstOrDefault(t => t.Type == "id").Value;
                        }

                        if (Claims.Any(t => t.Type == "client_id"))
                        {
                            sesstion.ClientId = Claims.FirstOrDefault(t => t.Type == "client_id").Value;
                        }

                        if (Claims.Any(t => t.Type == "roles"))
                        {
                            var roles = Claims.FirstOrDefault(t => t.Type == "roles").Value;
                            if (string.IsNullOrEmpty(roles))
                            {
                                sesstion.Roles = new string[] { "default" };
                            }
                            else
                            {
                                sesstion.Roles = SerializerHelper.FromJson<string[]>(roles);
                            }
                        }

                        if (Claims.Any(t => t.Type.Contains("givenname")))
                        {
                            // Claims.FirstOrDefault(t => t.Subject.Name);
                            sesstion.CnName = Claims.FirstOrDefault(t => t.Type.Contains("givenname")).Value;
                        }
                        if (Claims.Any(t => t.Type == "avatar"))
                        {
                            sesstion.Avatar = Claims.FirstOrDefault(t => t.Type == "avatar").Value;
                        }

                        if (Claims.Any(t => t.Type == "name"))
                        {
                            sesstion.Account = Claims.FirstOrDefault(t => t.Type == "name").Value;
                        }
                    }
                    return sesstion;
                }
                else
                {
                    return sesstion;
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex,ex.Message, "SYS");
                return new SesstionInfo();
            }

        }

        /// <summary>
        ///  返回SuccessResult
        /// </summary>
        protected virtual INPResult Success(string msg)
        {
            return new SuccessResult<String>(msg);
        }
        /// <summary>
        ///  返回SuccessResult
        /// </summary>
        protected virtual INPResult Success()
        {
            return new SuccessResult<string>(string.Empty);
        }

        /// <summary>
        ///  返回SuccessResult
        /// </summary>
        protected virtual SuccessResult<T> Success<T>(T obj)
        {
            return new SuccessResult<T>(obj);
        }

        /// <summary>
        ///  返回SuccessResult<T/>
        /// </summary>
        protected virtual SuccessResult<T> Success<T>(string msg, T obj) 
        {
            return new SuccessResult<T>(msg, obj);
        }

        /// <summary>
        /// 返回错误信息
        /// </summary>
        protected virtual ErrorResult<object> Error(NPlatformException ex)
        {
            return  Error<object>(ex.Message);
        }
        /// <summary>
        /// 返回错误信息
        /// </summary>
        protected virtual ErrorResult<object> Error(string msg,string modelName)
        {
            Logger.LogError(msg, modelName);
            var rst = new ErrorResult<object>($"{msg}", HttpStatusCode.InternalServerError);
            return rst;
        }
        /// <summary>
        /// 返回错误信息
        /// </summary>
        protected virtual ErrorResult<T> Error<T>(string msg, HttpStatusCode httpStatusCode= HttpStatusCode.InternalServerError
            , string modelName = null, NPlatform.NPlatformException ex=null)
        {
            Logger.LogError(ex,ex.Message, modelName);
            var rst = new ErrorResult<T>($"{msg}{"-->" + ex.Message}", httpStatusCode);
            return rst;
        }
        /// <summary>
        /// 树格式节点
        /// </summary>
        /// <typeparam name="T">TreeNode 类型</typeparam>
        /// <param name="nodes">树节点</param>
        /// <returns></returns>
        protected TreeResult<T> TreeData<T>(IEnumerable<T> nodes) where T: class
        {
            var trees = new TreeResult<T>();
            trees.AddRange(nodes);
            return trees;
        }

        /// <summary>
        /// 返回数据集合
        /// </summary>
        protected virtual ListResult<T> ListData<T>(IEnumerable<T> list, long total)
        {
            var content = new ListResult<T>(list, total);
            return content;
        }
        /// <summary>
        /// 返回数据集合
        /// </summary>
        protected virtual ListResult<T> ListData<T>(IEnumerable<T> list)
        {
            var content = new ListResult<T>(list, -1);
            return content;
        }

        /// <summary>
        ///  返回SuccessResult
        /// </summary>
        protected virtual SuccessResult<String> StrData(string msg)
        {
            return new SuccessResult<String>(msg);
        }
    }
}