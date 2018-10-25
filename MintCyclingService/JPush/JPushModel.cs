using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintCyclingService.JPush
{
    public class JPushModel
    {
    }
    ///// <summary>
    ///// 极光推送测试 输入参数模型
    ///// </summary>
    //public class Test_JPushMessage_PM
    //{
    //    public string ValidateCode { get; set; }


    //}
        


    /// <summary>
    /// 接收JPush返回值
    /// </summary>
    public class JpushMsg
    {
        private string sendno;//编号

        public string Sendno
        {
            get { return sendno; }
            set { sendno = value; }
        }
        private string msg_id;//信息编号

        public string Msg_id
        {
            get { return msg_id; }
            set { msg_id = value; }
        }
        private string errcode;//返回码

        public string Errcode
        {
            get { return errcode; }
            set { errcode = value; }
        }
        private string errmsg;//错误信息

        public string Errmsg
        {
            get { return errmsg; }
            set { errmsg = value; }
        }
    }

}
