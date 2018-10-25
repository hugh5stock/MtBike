using DTcms.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utility.LogHelper;

namespace MintCyclingWebApi
{
    public partial class UpgradeFile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string Mac = string.Empty;
            string Version = string.Empty;
            string Url = string.Empty;
 
             Mac= DTRequest.GetString("Mac");
             Version = DTRequest.GetString("Version");
             Url = DTRequest.GetString("Url");

            //日志
            LogHelper.Info("时间：" + DateTime.Now + ",Mac：" + Mac + ",Version：" + Version + ",Url:" + Url);




        }
    }
}