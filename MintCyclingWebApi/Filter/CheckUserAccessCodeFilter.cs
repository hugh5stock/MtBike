using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Utility.Common;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;
using MintCyclingService.AdminAccessCode;

namespace MintCyclingWebApi.Filter
{
    /// <summary>
    /// 检验用户AccessCode是否有效
    /// </summary>
    public class CheckUserAccessCodeFilter : ActionFilterAttribute
    {
        /// <summary>
        /// Action执行前运行
        /// </summary>
        /// <param name="actionContext">Action上下文</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var accesscode = Guid.Parse(actionContext.Request.Headers.GetValues(WebApiApplication.AccessCodeName).First());

            // 检查AccessCode是否有效
            var _userMemberAccessCodeRepo = AutoFacConfig.Container.Resolve<IAdminAccessCodeService>();
            _userMemberAccessCodeRepo.VeriftyAccessCode(accesscode);


        }
    }
}
