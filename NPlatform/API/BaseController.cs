using Microsoft.AspNetCore.Authorization;
using NPlatform.Result;

namespace NPlatform.API
{
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.AspNetCore.Mvc;
    using NPlatform.Consts;
    using NPlatform.Dto;
    using NPlatform.Infrastructure.Redis;
    using Org.BouncyCastle.Ocsp;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// controler 基类
    /// </summary>
    // [AllowAnonymous]
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// redis service
        /// </summary>
        [Autowired]
        public IRedisService _RedisService { get; set; }
        /// <summary>
        /// 全局配置信息
        /// </summary>
        [Autowired]
        protected IConfiguration Config { get; set; }

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
                return System.Text.Json.JsonSerializer.Deserialize<T>(postParamsString);
            }
        }

        /// <summary>
        /// 获取UI传递的js 数组参数 'Content-Type':'application/json' params:JSON.stringify(Array)
        /// </summary>
        /// <returns></returns>
        protected virtual async Task<string> GetRequestStrParamsAsync()
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
            if (sesstion == null)
            {
                var token = Request.Headers["Authorization"];
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
                            sesstion.Roles = System.Text.Json.JsonSerializer.Deserialize<string[]>(roles);
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

        // 直接移植的 mvc框架的Controller代码
        private ITempDataDictionary? _tempData;
        private ViewDataDictionary? _viewData;

        /// <summary>
        /// Gets or sets <see cref="ViewDataDictionary"/> used by <see cref="ViewResult"/> and <see cref="ViewBag"/>.
        /// </summary>
        /// <remarks>
        /// By default, this property is initialized when <see cref="Controllers.IControllerActivator"/> activates
        /// controllers.
        /// <para>
        /// This property can be accessed after the controller has been activated, for example, in a controller action
        /// or by overriding <see cref="OnActionExecuting(ActionExecutingContext)"/>.
        /// </para>
        /// <para>
        /// This property can be also accessed from within a unit test where it is initialized with
        /// <see cref="EmptyModelMetadataProvider"/>.
        /// </para>
        /// </remarks>
        [ViewDataDictionary]
        public ViewDataDictionary ViewData {
            get {
                if (_viewData == null) {
                    // This should run only for the controller unit test scenarios
                    _viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), ControllerContext.ModelState);
                }

                return _viewData!;
            }
            set {
                if (value == null) {
                    throw new ArgumentException(nameof(ViewData));
                }

                _viewData = value;
            }
        }

        /// <summary>
        /// Gets or sets <see cref="ITempDataDictionary"/> used by <see cref="ViewResult"/>.
        /// </summary>
        public ITempDataDictionary TempData {
            get {
                if (_tempData == null) {
                    var factory = HttpContext?.RequestServices?.GetRequiredService<ITempDataDictionaryFactory>();
                    _tempData = factory?.GetTempData(HttpContext);
                }

                return _tempData!;
            }
            set {
                if (value == null) {
                    throw new ArgumentNullException(nameof(value));
                }

                _tempData = value;
            }
        }


        /// <summary>
        ///  返回SuccessResult
        /// </summary>
        protected virtual SuccessResult<string> Success(string msg)
        {
            return new SuccessResult<string>(msg);
        }
        /// <summary>
        ///  返回SuccessResult
        /// </summary>
        protected virtual SuccessResult<string> Success()
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
        protected virtual ErrorResult<IDto> Error(string msg)
        {
            var rst = new ErrorResult<IDto>(msg);
            return rst;
        }

        /// <summary>
        /// 返回错误信息
        /// </summary>
        protected virtual ErrorResult<T> Error<T>(Exception ex)
        {
            var rst = new ErrorResult<T>(ex);
            return rst;
        }

        /// <summary>
        /// 树格式节点
        /// </summary>
        /// <typeparam name="T">TreeNode 类型</typeparam>
        /// <param name="nodes">树节点</param>
        /// <returns></returns>
        protected TreeResult<T> TreeData<T>(IEnumerable<T> nodes) where T : class
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
        protected virtual SuccessResult<string> StrData(string msg)
        {
            return new SuccessResult<string>(msg);
        }
    }
}