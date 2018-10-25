using MintCyclingService.Common;
using MintCyclingService.User;
using MintCyclingService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace MintCyclingWebApi.Filter
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class RequestCheck : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            try
            {
                var token = HttpContext.Current.Request.Headers.Get("UToken");
                Guid tag;
                if (Guid.TryParse(token, out tag)) //检查当前Token字符串是否有效
                {
                    if (CheckUserTicket(tag))
                    {
                        base.IsAuthorized(actionContext);
                    }
                    else
                    {
                        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.OK, new ResultModel
                        {    
                            //当前Token已失效
                            IsSuccess = false,
                            MsgCode = ResPrompt.UserTokenNotEixist,
                            Message = ResPrompt.UserTokenNotEixistMessage
                        });
                    }
                }
                else
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.OK, new ResultModel
                    {
                        //当前Token不合法
                        IsSuccess = false,
                        MsgCode = ResPrompt.UserTokenError,
                        Message = ResPrompt.UserTokenErrorMessage
                    });
                }
            }
            catch(Exception ex)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.OK, new ResultModel
                {
                    IsSuccess = false,
                    MsgCode = ResPrompt.UserTokenCheckError,
                    Message = ResPrompt.UserTokenCheckErrorMessage
                });
            }
        }

        private bool CheckUserTicket(Guid tk)
        {
            IUserInfoService userInfoService = new UserInfoService();
            var st = userInfoService.GetUserTokenExist(tk);
            return st.IsSuccess;
        }
    }
}