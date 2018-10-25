using MintCyclingService.Utils;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Utility.Common;

namespace MintCyclingWebApi.Filter
{
    /// <summary>
    /// WebApi异常捕获过滤器
    /// </summary>
    public class WebApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// 重写基类的异常处理方法
        /// </summary>
        /// <param name="actionExecutedContext">异常参数</param>
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var result = new ResultModel();

            //1.异常日志记录（正式项目里面一般是用log4net记录异常日志）
            //Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "——" +
            //    actionExecutedContext.Exception.GetType().ToString() + "：" + actionExecutedContext.Exception.Message + "——堆栈信息：" +
            //    actionExecutedContext.Exception.StackTrace);

            //2.返回调用方具体的异常信息

            if (actionExecutedContext.Exception is AccessCodeException)
            {
                result.IsSuccess = false;
                result.MsgCode = ResultCodeEnum.AccessCodeError;
                result.Message = ResultCodeEnum.AccessCodeErrorMessage;
                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.OK, result);
            }
            else if (actionExecutedContext.Exception is System.FormatException)
            {
                result.IsSuccess = false;
                result.MsgCode = ResultCodeEnum.SystemError;
                result.Message = actionExecutedContext.Exception.Message;
                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.OK, result);
            }           
            else
            {
                result.IsSuccess = false;
                result.MsgCode = ResultCodeEnum.SystemError;
                result.Message = actionExecutedContext.Exception.Message;
                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.OK, result);
            }



            base.OnException(actionExecutedContext);
        }
    }
}