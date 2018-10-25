using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace MtBikeAdminWebApi.Filter
{
    /// <summary>
    /// Http Header 过滤器, Swagger专用
    /// </summary>
    public class HttpHeaderFilter : IOperationFilter
    { 
        /// <summary>
        /// 应用
        /// </summary>
        /// <param name="operation">操作参数</param>
        /// <param name="schemaRegistry">结构注册</param>
        /// <param name="apiDescription">Api描述</param>
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation.parameters == null)
                operation.parameters = new List<Parameter>();

            ////var filterPipeline = apiDescription.ActionDescriptor.GetFilterPipeline(); //判断是否添加权限过滤器
            ////var isAuthorized = filterPipeline.Select(filterInfo => filterInfo.Instance).Any(filter => filter is IAuthorizationFilter); //判断是否允许匿名方法 

            var isNeedLogin = apiDescription.ActionDescriptor.GetCustomAttributes<CheckUserAccessCodeFilter>().Any(); //是否有验证用户标记

            if (isNeedLogin) //如果有验证标记则 多输出2个文本框(swagger form提交时会将这2个值放入header里)
            {
                operation.parameters.Add(new Parameter { name = "StampTime", @in = "header", description = "StampTime", required = false, type = "string" });
                operation.parameters.Add(new Parameter { name = "MD5", @in = "header", description = "MD5", required = false, type = "string" });
            }
        }
    }
}
