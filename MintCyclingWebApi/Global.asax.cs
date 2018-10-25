using Autofac;
using MintCyclingService.AdminAccessCode;
using MintCyclingService.Common;
using MintCyclingService.Cycling;
using MintCyclingService.Utils;
using MintCyclingWebApi.Filter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Utility.Common;
using Utility.LogHelper;

namespace MintCyclingWebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// AccessCode属性名
        /// </summary>
        public static string AccessCodeName = "AccessCode";
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AutoFacConfig.AutoFacRegister();
            //异常处理过滤器
            GlobalConfiguration.Configuration.Filters.Add(new WebApiExceptionFilterAttribute());
            //定时任务处理
            CreateTimer();

            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(System.IO.Path.Combine(HttpRuntime.AppDomainAppPath, "Config/log4net.config")));
        }

        /// <summary>
        /// 被请求时
        /// 在Angular.js中的H5页面调用Web api时跨域问题处理
        /// </summary>
        /// <param name="sender">请求者</param>
        /// <param name="e">参数</param>
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");

            if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
            {
                HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache");
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST");
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "AccessCode, content-type");
                HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "1728000");
                HttpContext.Current.Response.End();
            }
        }

        ///// <summary>
        ///// 根据AccessCode查询用户数据
        ///// </summary>
        ///// <returns></returns>
        //public static ResultModel GetCurAccessData()
        //{
        //    var tk = HttpContext.Current.Request.Headers.GetValues(AccessCodeName);
        //    var nonSq = new ResultModel { IsSuccess = false, MsgCode = ResPrompt.AccessCodeNotExisted, Message = ResPrompt.AccessCodeNotExistedMessage };
        //    if (tk == null || tk.Length == 0)
        //        return nonSq;
        //    var guid = tk.First();
        //    var st = Guid.Empty;
        //    if (!Guid.TryParse(guid, out st))
        //        return nonSq;
        //    IUserMemberAccessCodeService _userMemberAccessCodeRepo = AutoFacConfig.Container.Resolve<IUserMemberAccessCodeService>();
        //    var sk = _userMemberAccessCodeRepo.GetUserMemberGuidByAccessCode(st);
        //    return sk;
        //}

        /// <summary>
        /// 查询后台用户
        /// </summary>
        /// <returns></returns>
        public static ResultModel GetAdminUserData()
        {
            var tk = HttpContext.Current.Request.Headers.GetValues(AccessCodeName);
            var nonSq = new ResultModel { IsSuccess = false, MsgCode = ResPrompt.AccessCodeNotExisted, Message = ResPrompt.AccessCodeNotExistedMessage };
            if (tk == null || tk.Length == 0)
                return nonSq;
            var guid = tk.First();
            var st = Guid.Empty;
            if (!Guid.TryParse(guid, out st))
                return nonSq;
            IAdminAccessCodeService _adminAccessCodeRepo = AutoFacConfig.Container.Resolve<IAdminAccessCodeService>();
            var sk = _adminAccessCodeRepo.GetAdminGuidByAccessCodes(st);
            return sk;
        }



        /// <summary>
        /// 创建一个定时器
        /// </summary>
        protected void CreateTimer()
        {
            //设置定时器间隔时间
            string Interval = ConfigurationSettings.AppSettings["AutoLoanInterval"];
            //实例化定时器
            //System.Timers.Timer myTimer = new System.Timers.Timer(60000 * Convert.ToDouble(Interval));
            System.Timers.Timer myTimer = new System.Timers.Timer(Convert.ToDouble(Interval));

            myTimer.Elapsed += new System.Timers.ElapsedEventHandler(ReservationTaskMgr.AutoEvent);
            myTimer.Enabled = true;
            myTimer.AutoReset = true;
            //日志记录
            LogHelper.Info("处理用户预约超时定时器：当前创建时间：" + DateTime.Now + ",CreateTimer正在进行中");

            //FileLog.Log("处理用户预约超时定时器：当前创建时间：" + DateTime.Now + ",CreateTimer正在进行中", "TimerTaskLog");
        }



    }
}
