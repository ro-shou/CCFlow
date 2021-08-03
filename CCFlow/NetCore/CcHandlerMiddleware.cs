using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Text.RegularExpressions;
using BP.WF.HttpHandler;
using BP.Web;
using System.Text;
using BP.DA;
using Microsoft.Extensions.Logging;

namespace CCFlow
{
    public class CcHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CcHandlerMiddleware> _logger;

        /// <summary>
        /// 构造方法。
        /// 注：参数都是注入的。
        /// </summary>
        /// <param name="next"></param>
        /// <param name="optionsAccessor"></param>
        /// <param name="dataProtectionProvider"></param>
        /// <remarks>2019-7-29 zl</remarks>
        public CcHandlerMiddleware(RequestDelegate next, ILogger<CcHandlerMiddleware> logger)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the logic of the middleware.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <returns>A <see cref="Task"/> that completes when the middleware has completed processing.</returns>
        public async Task Invoke(HttpContext context)
        {
            if (context.Response.HasStarted)
                return;

            string path = context.Request.Path;

            // 首页 /
            if (path == "/")
            {
                if (DBAccess.IsExitsObject("WF_Flow") == false)
                    context.Response.Redirect("/WF/Admin/DBInstall.htm");
                else
                    context.Response.Redirect("/wwwroot/login.html");
                    //context.Response.Redirect("/WF/Admin/CCBPMDesigner/Login.htm");
                return;
            }

            // 动态执行页
            if (path.EndsWith(".ashx", StringComparison.OrdinalIgnoreCase))
            {
                // 获取 “Handler业务处理类”的Type
                Type ctrlType = null;
                if (path.EndsWith("WF/Comm/Handler.ashx", StringComparison.OrdinalIgnoreCase))
                {
                    ctrlType = typeof(BP.WF.HttpHandler.WF_Comm);
                }
                else if (path.EndsWith("WF/CCForm/Handler.ashx", StringComparison.OrdinalIgnoreCase))
                {
                    ctrlType = typeof(BP.WF.HttpHandler.WF_CCForm);
                }
                else if (path.EndsWith("WF/Admin/CCBPMDesigner/Handler.ashx", StringComparison.OrdinalIgnoreCase)
                      || path.EndsWith("WF/Admin/Portal/Handler.ashx", StringComparison.OrdinalIgnoreCase))
                {
                    ctrlType = typeof(BP.WF.HttpHandler.WF_Admin_CCBPMDesigner);
                }

                if (ctrlType == null)
                {
                    string errorMsg = "リクエストされたashxパスが間違っています！ WF/Comm/Handler.ashxまたはWF/CCForm/Handler.ashxまたはWF/CCBPMDesigner/Handler.ashxまたはWF/Admin/Portal/Handler.ashxのいずれかになります。。";
                    BP.DA.Log.DebugWriteError(errorMsg);

                    HttpContextHelper.Response.StatusCode = StatusCodes.Status404NotFound;
                    HttpContextHelper.ResponseWriteString(errorMsg, Encoding.UTF8);
                    _logger.LogError(errorMsg);
                    return;
                }

                DirectoryPageBase ctrl = null;

                try
                {
                    //创建 ctrl 对象, 获得业务实体类.
                    ctrl = Activator.CreateInstance(ctrlType) as DirectoryPageBase;

                    //执行方法返回json.
                    string data = ctrl.DoMethod(ctrl, ctrl.DoType);

                    //返回执行的结果.
                    if (data.StartsWith("err@", StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogError(data + Environment.NewLine + "リクエストされたURLは：" + context.Request.Path.ToUriComponent() + context.Request.QueryString);
                    }
                    
                    HttpContextHelper.Response.StatusCode = StatusCodes.Status200OK;
                    HttpContextHelper.ResponseWriteString(data, Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    string paras = "";
                    foreach (string key in HttpContextHelper.RequestQueryStringKeys)
                    {
                        paras += "@" + key + "=" + HttpContextHelper.RequestQueryString(key);
                    }

                    string err = "";
                    //返回执行错误的结果.
                    if (ex.InnerException != null)
                        err = "err@実行クラス[" + ctrlType.ToString() + "]，メソッド[" + ctrl.DoType + "]エラー \t\n @" + ex.InnerException.Message + " \t\n @技術的な情報:" + ex.StackTrace + " \t\n関連パラメーター:" + paras;
                    else
                        err = "err@実行クラス[" + ctrlType.ToString() + "]，メソッド[" + ctrl.DoType + "]エラー \t\n @" + ex.Message + " \t\n @技術的な情報:" + ex.StackTrace + " \t\n関連パラメーター:" + paras;

                    if (BP.Web.WebUser.No == null)
                        err = "err@ログイン時間が長すぎます。再度ログインしてください。:" + err;

                    //记录错误日志以方便分析.
                    BP.DA.Log.DebugWriteError(err);

                    HttpContextHelper.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    HttpContextHelper.ResponseWriteString(err, Encoding.UTF8);
                    _logger.LogError(err);
                }

                return;
            }

            // 其他资源，比如htm、js、jpg等文件
            await _next(context);
        }
    }
}
