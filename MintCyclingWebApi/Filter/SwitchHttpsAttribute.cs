using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MintCyclingWebApi.Filter
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class SwitchHttpsAttribute : System.Web.Mvc.RequireHttpsAttribute
    {
        /// <summary>
        /// 字段表示是否需要安全的https链接,默认不需要
        /// </summary>
        public bool RequireSecure = false;

        /// <summary>
        /// 重写验证方法,判断是否需要https,如果需要https,就交给父类的方法处理,如果不需要,就自己处理
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
        {
            if (RequireSecure)
            {
                //需要https,执行父类方法,转化为https
                base.OnAuthorization(filterContext);
            }
            else
            {
                //如果设置为非安全链接,即http,进入该区块
                //判断链接,如果为https,这转换为http
                if (filterContext.HttpContext.Request.IsSecureConnection)
                {
                    HandleNonHttpRequest(filterContext);
                }
            }
        }

        /// <summary>
        /// 重写处理链接方法,处理https请求,使其重定向http
        /// </summary>
        /// <param name="filterContext"></param>
        protected virtual void HandleNonHttpRequest(AuthorizationContext filterContext)
        {
            if (String.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {

                // 从web.config中获取https的端口
                string port = ":" + System.Configuration.ConfigurationManager.AppSettings["HttpPort"];

                // redirect to HTTP version of page
                string url = "http://" + filterContext.HttpContext.Request.Url.Host + port + filterContext.HttpContext.Request.RawUrl;

                //重定向
                filterContext.Result = new RedirectResult(url);
            }
        }

    }
}