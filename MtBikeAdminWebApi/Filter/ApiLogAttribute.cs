using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace HuRongClub.Admin.Filters
{


    /// <summary>
    /// 日志
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class ApiLogAttribute : ActionFilterAttribute
    {

        public ApiLogAttribute()
        {
            Order = 5;
        }

        //private readonly HuRongClub.BLL.YYD_APILog YYD_APILogBLL = new HuRongClub.BLL.YYD_APILog();
        //private DateTime _startTime;
        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    _startTime = DateTime.Now;
        //    base.OnActionExecuting(filterContext);
        //}

        ///// <summary>
        ///// 每个Action执行完毕后，执行该方法，该方法为日志的记录表。
        ///// 
        ///// </summary>
        ///// <param name="filterContext"></param>
        //public override void OnActionExecuted(ActionExecutedContext filterContext)
        //{
        //    SiteConfigBLL scBll = new SiteConfigBLL();
        //    Model.siteconfig sconfig = scBll.getConfig();
        //    if (sconfig.islog == 1)  //是否开启了“请求的日志”
        //    {
        //        DateTime now = DateTime.Now;
        //        DateTime beginDate = MyCommFun.Obj2DateTime(sconfig.logbegindate);
        //        DateTime endDate = MyCommFun.Obj2DateTime(sconfig.logenddate);

        //        if (now >= beginDate && now <= endDate)
        //        {   //如果在规定的时间内，则记录日志

        //            var log = new HuRongClub.Model.YYD_APILog
        //            {
        //                span_time = (int)(DateTime.Now - _startTime).TotalMilliseconds,
        //                url = filterContext.HttpContext.Request.Url.AbsoluteUri,
        //                http_method = filterContext.HttpContext.Request.HttpMethod,
        //                query_string = HttpUtility.UrlDecode(filterContext.HttpContext.Request.QueryString.ToString()),
        //                form = HttpUtility.UrlDecode(filterContext.HttpContext.Request.Form.ToString()),
        //                create_time = _startTime,
        //                addip = RequestCommon.GetIP(),
        //                user_id = SiteCommon.GetUserID(),
        //                system_id = sconfig.systemid,
        //                system_name = sconfig.systemname,
        //            };
        //            YYD_APILogBLL.Add(log);
        //        }
        //    }

           
        //    base.OnActionExecuted(filterContext);
        //}
    }
}