using System;

namespace Utility.LogHelper
{
    /// <summary>
    /// 站点配置实体类
    /// </summary>
    [Serializable]
    public partial class siteconfig
    {
        public siteconfig()
        { }

        private string _systemname = "";
        private int _systemid = 0;
        private int _isdebug = 0;
        private string _debugbegindate = "";
        private string _debugenddate = "";

        private int _islog = 0;
        private string _logbegindate = "";
        private string _logenddate = "";
        private bool _isClose = false;
        private bool _isTest = false;
        private string _templateUsers = "";
        private string _templateId = "";
        private string _appid = "";
        private string _appsecret = "";
        private string _returnurl = "";

        #region 控制调试基本信息==================================

        /// <summary>
        /// 日志里标志的系统的名称（每个系统都不要一样，不然在日志文件里无法识别）
        /// </summary>
        public string systemname
        {
            get { return _systemname; }
            set { _systemname = value; }
        }

        /// <summary>
        ///  日志里标志的系统的Id（每个系统都不要一样，不然在日志文件里无法识别）
        /// </summary>
        public int systemid
        {
            get { return _systemid; }
            set { _systemid = value; }
        }

        /// <summary>
        ///
        /// 站点是否被关闭，默认是未被关闭
        /// </summary>
        public bool isClose
        {
            get { return _isClose; }
            set { _isClose = value; }
        }

        /// <summary>
        /// 是否为测试状态
        /// </summary>
        public bool isTest
        {
            get { return _isTest; }
            set { _isTest = value; }
        }

        /// <summary>
        /// 调试是否打开：0未打开，1打开调试
        /// </summary>
        public int isdebug
        {
            get { return _isdebug; }
            set { _isdebug = value; }
        }

        /// <summary>
        /// debug开启时间：开始时间
        /// </summary>
        public string debugbegindate
        {
            get { return _debugbegindate; }
            set { _debugbegindate = value; }
        }

        /// <summary>
        /// debug开启时间：结束时间
        /// </summary>
        public string debugenddate
        {
            get { return _debugenddate; }
            set { _debugenddate = value; }
        }

        /// <summary>
        /// 日常的日志记录是否打开：0未打开，1打开调试
        /// </summary>
        public int islog
        {
            get { return _islog; }
            set { _islog = value; }
        }

        /// <summary>
        /// 日常的日志记录 开启时间：开始时间
        /// </summary>
        public string logbegindate
        {
            get { return _logbegindate; }
            set { _logbegindate = value; }
        }

        /// <summary>
        /// 日常的日志记录开启时间：结束时间
        /// </summary>
        public string logenddate
        {
            get { return _logenddate; }
            set { _logenddate = value; }
        }

        public string templateId
        {
            get { return _templateId; }
            set { _templateId = value; }
        }

        /// <summary>
        /// 模板接收人
        /// </summary>
        public string templateUsers
        {
            get { return _templateUsers; }
            set { _templateUsers = value; }
        }

        /// <summary>
        /// appid
        /// </summary>
        public string appid
        {
            get { return _appid; }
            set { _appid = value; }
        }

        /// <summary>
        /// 模板接收人
        /// </summary>
        public string appsecret
        {
            get { return _appsecret; }
            set { _appsecret = value; }
        }

        /// <summary>
        /// 回调地址
        /// </summary>
        public string returnurl
        {
            get { return _returnurl; }
            set { _returnurl = value; }
        }

        #endregion
    }
}