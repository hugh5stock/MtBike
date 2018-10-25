using DTcms.Common;
using System.Configuration;
using Utility.Common;

namespace Utility.LogHelper
{
    public class SiteConfigFun
    {
        private static object lockHelper = new object();

        /// <summary>
        /// 读取配置文件信息
        /// </summary>
        /// <returns></returns>
        public static siteconfig getConfig()
        {
            return (siteconfig)SerializationHelper.Load(typeof(siteconfig), Utils.GetXmlMapPath("websiteConfigpath"));
        }

        /// <summary>
        /// 写入站点配置文件
        /// </summary>
        public static  siteconfig saveConifg(siteconfig model)
        {
            lock (lockHelper)
            {
                SerializationHelper.Save(model, Utils.GetMapPath(Utils.getAppSettingValue("websiteConfigpath")));
            }
            return model;
        }

    }
}