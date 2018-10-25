using System.Text;

namespace Utility.LogHelper
{
    public class LogMessage
    {
        public LogMessage()
        {
            platName = "";
            Message = "";
            userid = 0;
            module = "";
            operating = "";
            flgValue = "";
            addip = "";
            remark = "";
        }

        /// <summary>
        ///1 平台
        /// </summary>
        public string platName { get; set; }

        /// <summary>
        ///2 日志内容
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///3 用户的uid
        /// </summary>
        public int userid { get; set; }

        /// <summary>
        ///4 模块
        /// </summary>
        public string module { get; set; }

        /// <summary>
        ///5 操作
        /// </summary>
        public string operating { get; set; }

        /// <summary>
        ///6 标志值
        /// </summary>
        public string flgValue { get; set; }

        /// <summary>
        ///7 备注
        /// </summary>
        public string remark { get; set; }

        /// <summary>
        ///8 ip地址
        /// </summary>
        public string addip { get; set; }

        public override string ToString()
        {
            //重写需要的输出。
            StringBuilder ret = new StringBuilder(this.Message);
            if (userid != 0)
            {
                ret.Append(" [user]=" + userid.ToString());
            }
            if (platName != null && platName.Trim() != "")
            {
                ret.Append(" [platName]=" + platName.Trim());
            }
            if (module != null && module.Trim() != "")
            {
                ret.Append(" [module]=" + module.Trim());
            }

            if (operating != null && operating.Trim() != "")
            {
                ret.Append(" [operating]=" + operating.Trim());
            }

            if (flgValue != null && flgValue.Trim() != "")
            {
                ret.Append(" [flgValue]=" + platName.Trim());
            }

            if (remark != null && remark.Trim() != "")
            {
                ret.Append(" [remark]=" + remark.Trim());
            }

            if (addip != null && addip.Trim() != "")
            {
                ret.Append(" [addip]=" + addip.Trim());
            }
            return ret.ToString();
        }
    }
}