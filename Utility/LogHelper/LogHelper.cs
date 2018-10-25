using DTcms.Common;
using log4net;
using System;
using System.Diagnostics;
using Utility.Common;
using Utility.LogHelper;

namespace Utility.LogHelper
{
    public static class LogHelper
    {
        private static string platName = "后台admin";

        #region error 错误、异常时候记录日志

        /// <summary>
        /// 错误、异常时候记录日志
        /// </summary>
        /// <param name="message"></param>
        public static void Error(string message)
        {
            LogManager.GetLogger(GetCurrentMethodFullName()).Error(message);
        }

        /// <summary>
        /// 错误、异常 记录日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Error(string message, Exception exception)
        {
            LogManager.GetLogger(GetCurrentMethodFullName()).Error(message, exception);
        }

        /// <summary>
        /// 错误、异常 记录日志
        /// </summary>
        /// <param name="message">记录内容</param>
        /// <param name="userid">userid</param>
        /// <param name="module">模块</param>
        /// <param name="operating">操作行为</param>
        /// <param name="flgValue">值</param>
        /// <param name="addip">ip</param>
        public static void Error(string message, int userid = 0, string module = "", string operating = "", string flgValue = "")
        {
            LogMessage lm = new LogMessage();

            lm.userid = userid;
            lm.platName = platName;
            lm.module = module;
            lm.operating = operating;
            lm.flgValue = flgValue;
            lm.addip = Utils.getIPAddress();
            lm.remark = message;
            LogManager.GetLogger(GetCurrentMethodFullName()).Error(lm);
        }

        /// <summary>
        /// 错误、异常 记录日志
        /// </summary>
        /// <param name="message">记录内容</param>
        /// <param name="userid">userid</param>
        /// <param name="module">模块</param>
        /// <param name="operating">操作行为</param>
        /// <param name="flgValue">值</param>
        /// <param name="addip">ip</param>
        public static void Error(string message, Exception exception, int userid = 0, string module = "", string operating = "", string flgValue = "")
        {
            LogMessage lm = new LogMessage();

            lm.userid = userid;
            lm.platName = platName;
            lm.module = module;
            lm.operating = operating;
            lm.flgValue = flgValue;
            lm.addip = Utils.getIPAddress();
            lm.remark = message;
            LogManager.GetLogger(GetCurrentMethodFullName()).Error(lm, exception);
        }

        #endregion

        #region info 请求，用户行为  记录日志

        /// <summary>
        /// 请求，用户行为  记录日志
        /// </summary>
        /// <param name="message"></param>
        public static void Info(string message)
        {
            LogManager.GetLogger(GetCurrentMethodFullName()).Info(message);
        }

        /// <summary>
        /// 请求，用户行为  记录日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public static void Info(string message, Exception ex)
        {
            LogManager.GetLogger(GetCurrentMethodFullName()).Info(message, ex);
        }

        /// <summary>
        /// 请求，用户行为  记录日志
        /// </summary>
        /// <param name="message">记录内容</param>
        /// <param name="userid">userid</param>
        /// <param name="module">模块</param>
        /// <param name="operating">操作行为</param>
        /// <param name="flgValue">值</param>
        /// <param name="addip">ip</param>
        public static void Info(string message, int userid = 0, string module = "", string operating = "", string flgValue = "")
        {
            LogMessage lm = new LogMessage();

            lm.userid = userid;
            lm.platName = platName;
            lm.module = module;
            lm.operating = operating;
            lm.flgValue = flgValue;
            lm.addip = Utils.getIPAddress();
            lm.remark = message;

            LogManager.GetLogger(GetCurrentMethodFullName()).Info(lm);
        }

        /// <summary>
        /// 请求，用户行为  记录日志
        /// </summary>
        /// <param name="message">记录内容</param>
        /// <param name="ex">异常</param>
        /// <param name="userid">userid</param>
        /// <param name="module">模块</param>
        /// <param name="operating">操作行为</param>
        /// <param name="flgValue">值</param>
        /// <param name="addip">ip</param>
        public static void Info(string message, Exception ex, int userid = 0, string module = "", string operating = "", string flgValue = "")
        {
            LogMessage lm = new LogMessage();

            lm.userid = userid;
            lm.platName = platName;
            lm.module = module;
            lm.operating = operating;
            lm.flgValue = flgValue;
            lm.addip = Utils.getIPAddress();
            lm.remark = message;
            LogManager.GetLogger(GetCurrentMethodFullName()).Info(lm, ex);
        }

        #endregion

        #region debug 调试记录日志

        /// <summary>
        /// 调试记录日志
        /// </summary>
        /// <param name="message"></param>
        public static void Debug(string message)
        {
            siteconfig siteconfig = SiteConfigFun.getConfig();
            if (siteconfig.isdebug == 1)
            {
                LogManager.GetLogger(GetCurrentMethodFullName()).Debug(message);
            }
        }

        /// <summary>
        ///  调试记录日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public static void Debug(string message, Exception ex)
        {
            siteconfig siteconfig = SiteConfigFun.getConfig();
            if (siteconfig.isdebug == 1)
            {
                LogManager.GetLogger(GetCurrentMethodFullName()).Debug(message, ex);
            }
        }

        /// <summary>
        /// 调试记录日志
        /// </summary>
        /// <param name="message">记录内容</param>
        /// <param name="userid">userid</param>
        /// <param name="module">模块</param>
        /// <param name="operating">操作行为</param>
        /// <param name="flgValue">值</param>
        /// <param name="addip">ip</param>
        public static void Debug(string message, int userid = 0, string module = "", string operating = "", string flgValue = "")
        {
            siteconfig siteconfig = SiteConfigFun.getConfig();
            if (siteconfig.isdebug == 1)
            {
                LogMessage lm = new LogMessage();

                lm.userid = userid;
                lm.platName = platName;
                lm.module = module;
                lm.operating = operating;
                lm.flgValue = flgValue;
                lm.addip = Utils.getIPAddress();
                lm.remark = message;
                LogManager.GetLogger(GetCurrentMethodFullName()).Debug(lm);
            }
        }

        #endregion

        #region warin 警告

        public static void Warn(string message)
        {
            LogManager.GetLogger(GetCurrentMethodFullName()).Warn(message);
        }

        public static void Warn(string message, Exception ex)
        {
            LogManager.GetLogger(GetCurrentMethodFullName()).Warn(message, ex);
        }

        /// <summary>
        /// 错误、异常 记录日志
        /// </summary>
        /// <param name="userid">userid</param>
        /// <param name="module">模块</param>
        /// <param name="operating">操作行为</param>
        /// <param name="flgValue">值</param>
        /// <param name="addip">ip</param>
        /// <param name="message">记录内容</param>
        public static void Warn(string message, int userid = 0, string module = "", string operating = "", string flgValue = "")
        {
            LogMessage lm = new LogMessage();

            lm.userid = userid;
            lm.platName = platName;
            lm.module = module;
            lm.operating = operating;
            lm.flgValue = flgValue;
            lm.addip = Utils.getIPAddress();
            lm.remark = message;
            LogManager.GetLogger(GetCurrentMethodFullName()).Warn(lm);
        }

        /// <summary>
        /// 错误、异常 记录日志
        /// </summary>
        ///  <param name="message">记录内容</param>
        /// <param name="userid">userid</param>
        /// <param name="module">模块</param>
        /// <param name="operating">操作行为</param>
        /// <param name="flgValue">值</param>
        /// <param name="addip">ip</param>
        public static void Warn(string message, Exception exception, int userid = 0, string module = "", string operating = "", string flgValue = "")
        {
            LogMessage lm = new LogMessage();
            lm.userid = userid;
            lm.platName = platName;
            lm.module = module;
            lm.operating = operating;
            lm.flgValue = flgValue;
            lm.addip = Utils.getIPAddress();
            lm.remark = message;
            LogManager.GetLogger(GetCurrentMethodFullName()).Warn(lm, exception);
        }

        #endregion

        private static string GetCurrentMethodFullName()
        {
            StackFrame frame;
            string MethodFunStr = "";
            string MethodFullNameStr = "";
            // bool flag;
            try
            {
                int num = 2;
                StackTrace stackTrace = new StackTrace();
                int length = stackTrace.GetFrames().Length;
                //do
                //{
                int num1 = num;
                // num = num1 + 1;
                frame = stackTrace.GetFrame(num1);
                MethodFunStr = frame.GetMethod().DeclaringType.ToString();
                // flag = (!MethodFunStr.EndsWith("Exception") ? false : num < length);
                //}
                //while (flag);
                string name = frame.GetMethod().Name;
                MethodFullNameStr = string.Concat(MethodFunStr, ".", name);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                MethodFullNameStr = exMessage.Substring(0, exMessage.Length > 200 ? 200 : exMessage.Length);

                LogManager.GetLogger("内部调试").Error("GetCurrentMethodFullName()方法报错啦！！！", ex);
            }
            return MethodFullNameStr;

            // return "TestName";
        }
    }
}