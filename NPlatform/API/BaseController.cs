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

        /// <summary>
        /// httpContext
        /// </summary>
        [Autowired]
        public IPlatformHttpContext Context { get; set; }


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
        /// Creates a <see cref="ViewResult"/> object that renders a view to the response.
        /// </summary>
        /// <returns>The created <see cref="ViewResult"/> object for the response.</returns>
        [NonAction]
        public virtual ViewResult View() {
            return View(viewName: null);
        }

        /// <summary>
        /// Creates a <see cref="ViewResult"/> object by specifying a <paramref name="viewName"/>.
        /// </summary>
        /// <param name="viewName">The name or path of the view that is rendered to the response.</param>
        /// <returns>The created <see cref="ViewResult"/> object for the response.</returns>
        [NonAction]
        public virtual ViewResult View(string? viewName) {
            return View(viewName, model: ViewData.Model);
        }

        /// <summary>
        /// Creates a <see cref="ViewResult"/> object by specifying a <paramref name="model"/>
        /// to be rendered by the view.
        /// </summary>
        /// <param name="model">The model that is rendered by the view.</param>
        /// <returns>The created <see cref="ViewResult"/> object for the response.</returns>
        [NonAction]
        public virtual ViewResult View(object? model) {
            return View(viewName: null, model: model);
        }

        /// <summary>
        /// Creates a <see cref="ViewResult"/> object by specifying a <paramref name="viewName"/>
        /// and the <paramref name="model"/> to be rendered by the view.
        /// </summary>
        /// <param name="viewName">The name or path of the view that is rendered to the response.</param>
        /// <param name="model">The model that is rendered by the view.</param>
        /// <returns>The created <see cref="ViewResult"/> object for the response.</returns>
        [NonAction]
        public virtual ViewResult View(string? viewName, object? model) {
            ViewData.Model = model;

            return new ViewResult() {
                ViewName = viewName,
                ViewData = ViewData,
                TempData = TempData
            };
        }

        /// <summary>
        /// Creates a <see cref="PartialViewResult"/> object that renders a partial view to the response.
        /// </summary>
        /// <returns>The created <see cref="PartialViewResult"/> object for the response.</returns>
        [NonAction]
        public virtual PartialViewResult PartialView() {
            return PartialView(viewName: null);
        }

        /// <summary>
        /// Creates a <see cref="PartialViewResult"/> object by specifying a <paramref name="viewName"/>.
        /// </summary>
        /// <param name="viewName">The name or path of the partial view that is rendered to the response.</param>
        /// <returns>The created <see cref="PartialViewResult"/> object for the response.</returns>
        [NonAction]
        public virtual PartialViewResult PartialView(string? viewName) {
            return PartialView(viewName, model: ViewData.Model);
        }

        /// <summary>
        /// Creates a <see cref="PartialViewResult"/> object by specifying a <paramref name="model"/>
        /// to be rendered by the partial view.
        /// </summary>
        /// <param name="model">The model that is rendered by the partial view.</param>
        /// <returns>The created <see cref="PartialViewResult"/> object for the response.</returns>
        [NonAction]
        public virtual PartialViewResult PartialView(object? model) {
            return PartialView(viewName: null, model: model);
        }

        /// <summary>
        /// Creates a <see cref="PartialViewResult"/> object by specifying a <paramref name="viewName"/>
        /// and the <paramref name="model"/> to be rendered by the partial view.
        /// </summary>
        /// <param name="viewName">The name or path of the partial view that is rendered to the response.</param>
        /// <param name="model">The model that is rendered by the partial view.</param>
        /// <returns>The created <see cref="PartialViewResult"/> object for the response.</returns>
        [NonAction]
        public virtual PartialViewResult PartialView(string? viewName, object? model) {
            ViewData.Model = model;

            return new PartialViewResult() {
                ViewName = viewName,
                ViewData = ViewData,
                TempData = TempData
            };
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
        protected virtual FailResult<IDto> Fail(string msg)
        {
            var rst = new FailResult<IDto>(msg);
            return rst;
        }

        /// <summary>
        /// 返回错误信息
        /// </summary>
        protected virtual FailResult<T> Fail<T>(Exception ex)
        {
            var rst = new FailResult<T>(ex);
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