using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;

namespace MtBikeAdminWebApi
{
    /// <summary>
    /// AutoFac配置类
    /// </summary>
    public class AutoFacConfig
    {
        /// <summary>
        /// 声明Ioc容器
        /// </summary>
        public static IContainer Container { get; set; }

        /// <summary>
        /// AutoFac注册
        /// </summary>
        public static void AutoFacRegister()
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(Assembly.Load("MintCyclingService")).
            Where(t => t.Name.EndsWith("Service")).
            AsImplementedInterfaces();



            Container = builder.Build();
        }
    }
}
