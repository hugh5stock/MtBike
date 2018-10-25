using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

namespace MintCyclingService.Cycling
{
    /// <summary>
    /// 定时任务
    /// </summary>
  public  class ReservationTaskMgr
    {
        public ReservationTaskMgr()
        {
        }


        public static void AutoEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            DateTime now = DateTime.Now;
            //自动处理超时数据
            //if (now.Hour >= 15)
            //{
            //    DTcms.Common.Utils.HttpGet(System.Configuration.ConfigurationManager.AppSettings["TaskTimerUrl"].ToString());
            //    FileLog.Log("定时任务：当前执行时间："+DateTime.Now+ ",自动处理超时数据完成", "TimerTaskLog");
            //}
            //调用预约超时自动处理接口
            Utility.Common.Utils.HttpGets(System.Configuration.ConfigurationManager.AppSettings["TaskTimerUrl"].ToString());


        }


    }
}
